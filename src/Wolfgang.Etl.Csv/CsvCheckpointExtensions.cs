using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Syntactic sugar for the resumable-extraction pattern: atomically persist and read back a
/// record counter, and resume a <see cref="CsvExtractor{TRecord}"/> from it via
/// <see cref="CsvExtractor{TRecord}.SkipRecordCount"/>.
/// </summary>
/// <remarks>
/// This is <b>not</b> a resumable-extraction feature — it is thin sugar over the existing
/// <see cref="CsvExtractor{TRecord}.SkipRecordCount"/> control. All the policy (when to
/// checkpoint, where to persist, exactly-once vs at-least-once semantics) remains the
/// caller's responsibility. The one mechanical piece worth not hand-rolling is the atomic
/// write, so a crash mid-checkpoint never leaves a torn counter file.
/// </remarks>
public static class CsvCheckpointExtensions
{
    /// <summary>
    /// Reads a 32-bit integer counter from <paramref name="path"/>, returning <c>0</c> when the
    /// file does not exist.
    /// </summary>
    /// <param name="path">The checkpoint file path.</param>
    /// <param name="token">A token to observe while reading.</param>
    /// <returns>The persisted count, or <c>0</c> when the file is absent.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or whitespace.</exception>
    /// <exception cref="FormatException">The file exists but does not contain a valid integer, or is too large to be one.</exception>
    public static async ValueTask<int> ReadCheckpointAsync(string path, CancellationToken token = default)
    {
        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path must not be empty or whitespace.", nameof(path));
        }

        token.ThrowIfCancellationRequested();

        if (!File.Exists(path))
        {
            return 0;
        }

        string text;
        // FileShare.Delete as well as Read: on Windows the atomic replace in WriteCheckpointAsync
        // renames/replaces the target, which fails with a sharing violation if a concurrent reader
        // holds it without delete-sharing. The reader keeps its snapshot, so no torn read.
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete, 4096, useAsync: true))
        {
            // A checkpoint holds a single small integer. Cap the size before allocating so a
            // corrupt or accidentally-huge file surfaces as a clean FormatException instead of an
            // int-overflow cast or a giant allocation. (int.MinValue is 11 chars; 64 is generous.)
            const int maxCheckpointBytes = 64;
            if (stream.Length > maxCheckpointBytes)
            {
                throw new FormatException
                (
                    $"Checkpoint file '{path}' is {stream.Length} bytes — too large to be a checkpoint integer."
                );
            }

            // Read in one shot via the stream (not StreamReader.ReadToEndAsync) so the token is
            // forwarded on every TFM.
            var length = (int)stream.Length;
            var buffer = new byte[length];
            var read = 0;
            while (read < length)
            {
                var n = await stream.ReadAsync(buffer, read, length - read, token).ConfigureAwait(false);
                if (n == 0)
                {
                    break;
                }

                read += n;
            }

            text = Encoding.UTF8.GetString(buffer, 0, read);
        }

        if (!int.TryParse(text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            throw new FormatException($"Checkpoint file '{path}' does not contain a valid integer.");
        }

        return count;
    }


    /// <summary>
    /// Writes <paramref name="count"/> to <paramref name="path"/> atomically: the value is written
    /// to <c>path + ".tmp"</c> and then renamed over <paramref name="path"/>. A crash mid-write
    /// leaves either the previous value intact or the new value — never a partial write.
    /// </summary>
    /// <param name="path">The checkpoint file path.</param>
    /// <param name="count">The count to persist.</param>
    /// <param name="token">A token to observe while writing.</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or whitespace.</exception>
    public static async ValueTask WriteCheckpointAsync(string path, int count, CancellationToken token = default)
    {
        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path must not be empty or whitespace.", nameof(path));
        }

        token.ThrowIfCancellationRequested();

        var temporaryPath = path + ".tmp";
        var bytes = Encoding.UTF8.GetBytes(count.ToString(CultureInfo.InvariantCulture));

        using (var stream = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
        {
            await stream.WriteAsync(bytes, 0, bytes.Length, token).ConfigureAwait(false);
            await stream.FlushAsync(token).ConfigureAwait(false);
        }

        // Atomic replace. File.Move's overwrite overload is only on the modern runtime targets;
        // on net462 / netstandard fall back to File.Replace (atomic when the destination exists)
        // or a plain Move for the very first write. If the replace itself fails (destination
        // locked, permissions), clean up the temp file so the "no stray .tmp" story holds.
        try
        {
#if NET8_0_OR_GREATER
            File.Move(temporaryPath, path, overwrite: true);
#else
            if (File.Exists(path))
            {
                File.Replace(temporaryPath, path, destinationBackupFileName: null);
            }
            else
            {
                File.Move(temporaryPath, path);
            }
#endif
        }
        catch (IOException)
        {
            TryDeleteTemporaryFile(temporaryPath);
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            TryDeleteTemporaryFile(temporaryPath);
            throw;
        }
    }


    private static void TryDeleteTemporaryFile(string temporaryPath)
    {
        try
        {
            File.Delete(temporaryPath);
        }
        catch (IOException)
        {
            // Best effort — leave any residue for the OS / the next write to reclaim.
        }
        catch (UnauthorizedAccessException)
        {
            // Best effort — leave any residue for the OS / the next write to reclaim.
        }
    }


    /// <summary>
    /// Reads the checkpoint at <paramref name="path"/> and sets
    /// <see cref="CsvExtractor{TRecord}.SkipRecordCount"/> on <paramref name="extractor"/> to that
    /// value, so the next extraction resumes past the already-acknowledged records.
    /// </summary>
    /// <typeparam name="TRecord">The extractor's record type.</typeparam>
    /// <param name="extractor">The extractor to resume.</param>
    /// <param name="path">The checkpoint file path.</param>
    /// <param name="token">A token to observe while reading.</param>
    /// <returns>The loaded count, so the caller can keep acknowledging forward.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="extractor"/> or <paramref name="path"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or whitespace.</exception>
    /// <exception cref="FormatException">The checkpoint file exists but does not contain a valid integer.</exception>
    public static async ValueTask<int> ResumeFromCheckpointAsync<TRecord>
    (
        this CsvExtractor<TRecord> extractor,
        string path,
        CancellationToken token = default
    )
        where TRecord : notnull
    {
        if (extractor is null)
        {
            throw new ArgumentNullException(nameof(extractor));
        }

        var count = await ReadCheckpointAsync(path, token).ConfigureAwait(false);
        extractor.SkipRecordCount = count;
        return count;
    }
}
