using System;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Finds the field of a malformed record and counts the lines a record spans. CsvHelper 34 reports a
/// malformed record as a whole and the line it starts on; <see cref="CsvBadDataInfo"/> and the line
/// numbers the extractor reports keep CsvHelper 33's field and end-line detail.
/// </summary>
internal static class CsvMalformedField
{
    /// <summary>
    /// The 0-based index of the first malformed field of <paramref name="rawRecord"/>: a quote inside
    /// an unquoted field, text after a closing quote, or a quote that is never closed. -1 when no field
    /// is malformed (a field-count mismatch).
    /// </summary>
    public static int Find(string rawRecord, char separator, char quote, out string field)
    {
        var index = 0;
        var start = 0;
        var position = 0;

        while (position <= rawRecord.Length)
        {
            var end = position < rawRecord.Length && rawRecord[position] == quote
                ? ScanQuoted(rawRecord, position, separator, quote, out var malformed)
                : ScanUnquoted(rawRecord, position, separator, quote, out malformed);

            if (malformed)
            {
                field = rawRecord.Substring(start, end - start);
                return index;
            }

            index++;
            position = end + 1;
            start = position;
        }

        field = string.Empty;
        return -1;
    }



    /// <summary>The number of line breaks (CR LF, LF or a lone CR) in <paramref name="text"/>.</summary>
    public static int CountLineBreaks(string text)
    {
        var count = 0;
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n' || (text[i] == '\r' && (i + 1 == text.Length || text[i + 1] != '\n')))
            {
                count++;
            }
        }

        return count;
    }



    /// <summary>The number of line breaks (CR LF, LF or a lone CR) in UTF-8 <paramref name="bytes"/>.</summary>
    public static int CountLineBreaks(ReadOnlySpan<byte> bytes)
    {
        var count = 0;
        for (var i = 0; i < bytes.Length; i++)
        {
            if (bytes[i] == (byte)'\n' || (bytes[i] == (byte)'\r' && (i + 1 == bytes.Length || bytes[i + 1] != (byte)'\n')))
            {
                count++;
            }
        }

        return count;
    }



    // From an opening quote to the separator after the field (or the end); malformed when the quote is
    // never closed or text follows the closing quote.
    private static int ScanQuoted(string raw, int position, char separator, char quote, out bool malformed)
    {
        var i = position + 1;
        while (i < raw.Length)
        {
            if (raw[i] != quote)
            {
                i++;
                continue;
            }

            if (i + 1 < raw.Length && raw[i + 1] == quote)
            {
                i += 2;
                continue;
            }

            // The closing quote: the field must end here.
            var next = i + 1;
            malformed = next < raw.Length && raw[next] != separator;
            return malformed ? EndOfField(raw, next, separator) : next;
        }

        malformed = true;
        return raw.Length;
    }



    // To the separator after the field (or the end); malformed when the field holds a quote.
    private static int ScanUnquoted(string raw, int position, char separator, char quote, out bool malformed)
    {
        var end = EndOfField(raw, position, separator);
        malformed = raw.IndexOf(quote, position, end - position) >= 0;
        return end;
    }



    private static int EndOfField(string raw, int position, char separator)
    {
        var end = raw.IndexOf(separator, position);
        return end < 0 ? raw.Length : end;
    }
}
