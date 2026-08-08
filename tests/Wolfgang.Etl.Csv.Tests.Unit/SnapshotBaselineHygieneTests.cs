using System;
using System.IO;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Guards the Verify snapshot baselines against the UTF-8 BOM that Verify writes when it
/// accepts a snapshot (Verify 31.x exposes no BOM-less file-encoding setting). A re-accepted
/// baseline would silently reintroduce the BOM and drift from the repository's
/// <c>charset = utf-8</c> (no BOM) <c>.editorconfig</c> rule, so this test fails until the
/// offending baseline is re-saved without a BOM.
/// </summary>
public class SnapshotBaselineHygieneTests
{
    [Fact]
    public void Verify_snapshot_baselines_are_stored_without_a_utf8_bom()
    {
        var snapshots = LocateSnapshotsDirectory();
        var baselines = Directory.GetFiles(snapshots, "*.verified.txt");

        Assert.NotEmpty(baselines);

        foreach (var baseline in baselines)
        {
            Assert.False
            (
                StartsWithUtf8Bom(baseline),
                $"'{Path.GetFileName(baseline)}' starts with a UTF-8 BOM. Verify re-adds one on accept; " +
                "re-save the baseline without a BOM (the repository's .editorconfig mandates charset = utf-8)."
            );
        }
    }


    // Walk up from the test assembly's location (always present at runtime) to the project
    // directory that holds Snapshots/. Deterministic in CI, unlike [CallerFilePath], which
    // bakes in the compile-time source path.
    private static string LocateSnapshotsDirectory()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "Snapshots");
            if (Directory.Exists(candidate) && Directory.GetFiles(candidate, "*.verified.txt").Length > 0)
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException
        (
            "Could not locate the Snapshots directory walking up from " + AppContext.BaseDirectory
        );
    }


    private static bool StartsWithUtf8Bom(string path)
    {
        using var stream = File.OpenRead(path);
        return stream.ReadByte() == 0xEF
            && stream.ReadByte() == 0xBB
            && stream.ReadByte() == 0xBF;
    }
}
