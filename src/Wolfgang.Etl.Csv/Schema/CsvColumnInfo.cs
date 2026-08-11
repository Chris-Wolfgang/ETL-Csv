using System;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// A single column of an inferred <see cref="CsvSchema"/>: its name, position, best-guess CLR type, and
/// (for dates) the format string that matched every sampled value.
/// </summary>
public sealed record CsvColumnInfo
{
    /// <summary>The column name — the header value, or <c>ColumnN</c> when the source has no header.</summary>
    public string Name { get; init; } = string.Empty;



    /// <summary>The 0-based column index.</summary>
    public int Index { get; init; }



    /// <summary>
    /// The most-restrictive CLR type every sampled value parsed as: one of <c>bool</c>, <c>int</c>,
    /// <c>long</c>, <c>decimal</c>, <see cref="DateTime"/>, <see cref="Guid"/>, or <c>string</c> (the fallback).
    /// </summary>
    public Type InferredType { get; init; } = typeof(string);



    /// <summary>Whether any sampled value was empty or whitespace.</summary>
    public bool Nullable { get; init; }



    /// <summary>
    /// For a <see cref="DateTime"/> column, a format string that <c>ParseExact</c>-matched every sampled
    /// value, when one was found; otherwise <c>null</c>.
    /// </summary>
    public string? Format { get; init; }
}
