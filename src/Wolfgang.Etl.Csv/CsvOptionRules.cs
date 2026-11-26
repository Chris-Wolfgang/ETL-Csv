using System;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// The limits CsvHelper 34 puts on the dialect options the extractor and loader pass to it: the
/// separator, quote and comment are single ASCII characters, and a quote inside a quoted field is
/// escaped by doubling it (RFC 4180), so <c>Escape</c> must be the quote character.
/// </summary>
internal static class CsvOptionRules
{
    /// <summary>Checks the delimiter, quote and escape of an options record.</summary>
    /// <exception cref="ArgumentException">One of them is outside CsvHelper 34's limits.</exception>
    public static void Validate(string? delimiter, char quote, char escape, string paramName)
    {
        _ = ToSeparator(delimiter, paramName);
        _ = ToQuote(quote, escape, paramName);
    }



    /// <summary>The separator character of <paramref name="delimiter"/>.</summary>
    /// <exception cref="ArgumentException"><paramref name="delimiter"/> is not one ASCII character.</exception>
    public static char ToSeparator(string? delimiter, string paramName = "Delimiter")
    {
        if (string.IsNullOrEmpty(delimiter))
        {
            throw new ArgumentException("Delimiter must not be null or empty.", paramName);
        }

        if (delimiter!.Length != 1 || delimiter[0] > 127)
        {
            throw new ArgumentException
            (
                $"Delimiter must be a single ASCII character; '{delimiter}' is not. CsvHelper 34 does not support multi-character or non-ASCII separators.",
                paramName
            );
        }

        return delimiter[0];
    }



    /// <summary>The quote character, checked against <paramref name="escape"/>.</summary>
    /// <exception cref="ArgumentException">
    /// <paramref name="quote"/> is not ASCII, or <paramref name="escape"/> is neither the same character
    /// nor the default <c>'"'</c>.
    /// </exception>
    public static char ToQuote(char quote, char escape, string paramName = "Quote")
    {
        _ = ToAscii(quote, paramName);

        // The default Escape ('"') is accepted with any Quote: it meant "double the quote" in
        // CsvHelper 33's default configuration, which is what RFC 4180 mode does.
        if (escape != quote && escape != '"')
        {
            throw new ArgumentException
            (
                $"Escape ('{escape}') must be the Quote character ('{quote}') or the default '\"'. CsvHelper 34 reads and writes RFC 4180 quoting, where a quote inside a quoted field is escaped by doubling it.",
                paramName
            );
        }

        return quote;
    }



    /// <summary>Returns <paramref name="newLine"/> when CsvHelper 34 can write it.</summary>
    /// <exception cref="ArgumentException">
    /// <paramref name="newLine"/> is empty, or not one of <c>"\r\n"</c>, <c>"\n"</c> and <c>"\r"</c>.
    /// </exception>
    public static string ToNewLine(string? newLine, string paramName = "NewLine")
    {
        if (string.IsNullOrEmpty(newLine))
        {
            throw new ArgumentException("NewLine must not be null or empty.", paramName);
        }

        if (newLine is not ("\r\n" or "\n" or "\r"))
        {
            throw new ArgumentException
            (
                "NewLine must be \"\\r\\n\", \"\\n\" or \"\\r\": CsvHelper 34 writes no other line ending.",
                paramName
            );
        }

        return newLine;
    }



    /// <summary>Returns <paramref name="value"/> when it is an ASCII character.</summary>
    /// <exception cref="ArgumentException"><paramref name="value"/> is not ASCII.</exception>
    public static char ToAscii(char value, string paramName)
    {
        if (value > 127)
        {
            throw new ArgumentException($"'{value}' must be an ASCII character for CsvHelper 34.", paramName);
        }

        return value;
    }
}
