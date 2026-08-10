using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// A CSV shape inferred from a sample: the ordered columns with their inferred types, plus the delimiter,
/// header flag, and encoding detected while reading. Produce one with <see cref="InferAsync"/>, then feed
/// it to an existing extractor via <see cref="ToColumnMaps"/>, or persist it as JSON.
/// </summary>
public sealed record CsvSchema
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private static readonly string[] CandidateDelimiters = { ",", "\t", "|", ";" };

    private static readonly string[] CandidateDateFormats =
    {
        "yyyy-MM-dd",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-dd HH:mm:ss",
        "MM/dd/yyyy",
        "dd/MM/yyyy",
        "yyyy/MM/dd",
        "yyyyMMdd",
    };



    /// <summary>The inferred columns, in source order.</summary>
    public IReadOnlyList<CsvColumnInfo> Columns { get; init; } = Array.Empty<CsvColumnInfo>();



    /// <summary>The delimiter used (supplied or auto-detected).</summary>
    public string Delimiter { get; init; } = ",";



    /// <summary>Whether the sample was read with a header row.</summary>
    public bool HasHeaderRecord { get; init; } = true;



    /// <summary>The encoding detected from the reader (its <c>CurrentEncoding</c>), or UTF-8.</summary>
    public Encoding Encoding { get; init; } = Encoding.UTF8;



    /// <summary>
    /// Reads up to <see cref="CsvSchemaInferenceOptions.SampleRows"/> rows from <paramref name="reader"/> and
    /// infers the column names, types, and date formats.
    /// </summary>
    /// <param name="reader">The reader to sample. Its <c>CurrentEncoding</c> (if a <see cref="StreamReader"/>) becomes the schema's encoding.</param>
    /// <param name="options">Inference options. When <c>null</c>, defaults are used.</param>
    /// <param name="cancellationToken">A token to cancel the read.</param>
    /// <returns>The inferred <see cref="CsvSchema"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="reader"/> is <c>null</c>.</exception>
    public static async Task<CsvSchema> InferAsync
    (
        TextReader reader,
        CsvSchemaInferenceOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        if (reader is null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        options ??= new CsvSchemaInferenceOptions();

        var lines = await BufferSampleLinesAsync(reader, options, cancellationToken).ConfigureAwait(false);
        var encoding = reader is StreamReader streamReader ? streamReader.CurrentEncoding : Encoding.UTF8;
        var delimiter = options.Delimiter ?? DetectDelimiter(lines);
        var rows = await ParseRowsAsync(lines, delimiter, options.Culture).ConfigureAwait(false);

        return new CsvSchema
        {
            Columns = BuildColumns(rows, options),
            Delimiter = delimiter,
            HasHeaderRecord = options.HasHeaderRecord,
            Encoding = encoding,
        };
    }



    /// <summary>
    /// Projects the schema to a <see cref="CsvColumnMap"/> collection that binds each inferred column, by
    /// index, to a property of the same name — ready to assign to a <see cref="CsvExtractor{TRecord}.ColumnMaps"/>.
    /// </summary>
    public IReadOnlyList<CsvColumnMap> ToColumnMaps() =>
        Columns
            .Select(c => new CsvColumnMap(c.Name) { Index = c.Index })
            .ToArray();



    /// <summary>Serializes the schema to JSON. Inferred types persist as short tokens and encoding as its web name.</summary>
    [RequiresUnreferencedCode("JSON serialization of CsvSchema uses System.Text.Json reflection and is not trim/AOT-safe. Schema inference itself is unaffected.")]
    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions);



    /// <summary>Deserializes a schema previously produced by <see cref="ToJson"/>.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="json"/> is null.</exception>
    /// <exception cref="FormatException"><paramref name="json"/> does not represent a schema.</exception>
    [RequiresUnreferencedCode("JSON deserialization of CsvSchema uses System.Text.Json reflection and is not trim/AOT-safe. Schema inference itself is unaffected.")]
    public static CsvSchema FromJson(string json)
    {
        if (json is null)
        {
            throw new ArgumentNullException(nameof(json));
        }

        return JsonSerializer.Deserialize<CsvSchema>(json, JsonOptions)
            ?? throw new FormatException("The JSON did not represent a CsvSchema.");
    }



    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new CsvClrTypeJsonConverter());
        options.Converters.Add(new CsvEncodingJsonConverter());
        return options;
    }



    private static async Task<List<string>> BufferSampleLinesAsync
    (
        TextReader reader,
        CsvSchemaInferenceOptions options,
        CancellationToken cancellationToken
    )
    {
        var maxLines = options.HasHeaderRecord ? options.SampleRows + 1 : options.SampleRows;
        var lines = new List<string>();

        while (lines.Count < maxLines)
        {
            cancellationToken.ThrowIfCancellationRequested();
#if NET7_0_OR_GREATER
            var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
#else
            var line = await reader.ReadLineAsync().ConfigureAwait(false);
#endif
            if (line is null)
            {
                break;
            }

            lines.Add(line);
        }

        return lines;
    }



    private static async Task<List<string[]>> ParseRowsAsync(List<string> lines, string delimiter, CultureInfo culture)
    {
        var config = new CsvConfiguration(culture)
        {
            Delimiter = delimiter,
            HasHeaderRecord = false,
            DetectColumnCountChanges = false,
            MissingFieldFound = null,
            BadDataFound = null,
        };

        var rows = new List<string[]>();
        using var stringReader = new StringReader(string.Join("\n", lines));
        using var parser = new CsvParser(stringReader, config);
        while (await parser.ReadAsync().ConfigureAwait(false))
        {
            rows.Add(parser.Record ?? Array.Empty<string>());
        }

        return rows;
    }



    private static List<CsvColumnInfo> BuildColumns(List<string[]> rows, CsvSchemaInferenceOptions options)
    {
        var header = options.HasHeaderRecord && rows.Count > 0 ? rows[0] : null;
        var dataStart = header is null ? 0 : 1;
        var columnCount = header?.Length ?? (rows.Count > 0 ? rows.Max(r => r.Length) : 0);

        var columns = new List<CsvColumnInfo>(columnCount);
        for (var c = 0; c < columnCount; c++)
        {
            var nonEmpty = new List<string>();
            var nullable = false;
            for (var r = dataStart; r < rows.Count; r++)
            {
                var row = rows[r];
                var value = c < row.Length ? row[c] : string.Empty;
                if (string.IsNullOrWhiteSpace(value))
                {
                    nullable = true;
                }
                else
                {
                    nonEmpty.Add(value);
                }
            }

            var type = ClassifyColumn(nonEmpty, options.Culture);
            columns.Add(new CsvColumnInfo
            {
                Name = header is not null && c < header.Length ? header[c] : $"Column{c + 1}",
                Index = c,
                InferredType = type,
                Nullable = nullable,
                Format = type == typeof(DateTime) ? DetectDateFormat(nonEmpty, options.Culture) : null,
            });
        }

        return columns;
    }



    private static Type ClassifyColumn(IReadOnlyList<string> nonEmptyValues, CultureInfo culture)
    {
        if (nonEmptyValues.Count == 0)
        {
            return typeof(string);
        }

        foreach (var candidate in CsvInferredTypes.Ladder)
        {
            if (nonEmptyValues.All(v => TryParseAs(v, candidate, culture)))
            {
                return candidate;
            }
        }

        return typeof(string);
    }



    private static bool TryParseAs(string value, Type type, CultureInfo culture)
    {
        if (type == typeof(bool)) { return bool.TryParse(value, out _); }
        if (type == typeof(int)) { return int.TryParse(value, NumberStyles.Integer, culture, out _); }
        if (type == typeof(long)) { return long.TryParse(value, NumberStyles.Integer, culture, out _); }
        if (type == typeof(decimal)) { return decimal.TryParse(value, NumberStyles.Number, culture, out _); }
        if (type == typeof(DateTime)) { return DateTime.TryParse(value, culture, DateTimeStyles.None, out _); }

        // The ladder's last rung — ClassifyColumn only ever passes a ladder type here.
        return Guid.TryParse(value, out _);
    }



    private static string? DetectDateFormat(IReadOnlyList<string> values, CultureInfo culture)
    {
        foreach (var format in CandidateDateFormats)
        {
            if (values.All(v => DateTime.TryParseExact(v, format, culture, DateTimeStyles.None, out _)))
            {
                return format;
            }
        }

        return null;
    }



    private static string DetectDelimiter(IReadOnlyList<string> lines)
    {
        var first = lines.FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
        if (first is null)
        {
            return ",";
        }

        var best = ",";
        var bestCount = 0;
        foreach (var candidate in CandidateDelimiters)
        {
            var count = first.Count(ch => ch == candidate[0]);
            if (count > bestCount)
            {
                bestCount = count;
                best = candidate;
            }
        }

        return best;
    }
}
