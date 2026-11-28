using System;
using System.ComponentModel;
using System.Globalization;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Converts field text to member values and back. CsvHelper 34 maps only the types its source
/// generator or a hand-written mapper describes, so the library converts the values of its
/// reflection-mapped records itself, with the rules CsvHelper 33's default type converters used:
/// the current culture, an empty field as <see langword="null"/> for nullable and reference types,
/// <c>Format</c> as the exact parse format and the write format.
/// </summary>
internal static class CsvValueConverter
{
    /// <summary>
    /// Converts <paramref name="text"/> to a value of <paramref name="type"/>.
    /// </summary>
    /// <exception cref="FormatException">The text is not a valid value of the type.</exception>
    public static object? Parse(string text, Type type, string? format, CultureInfo culture)
    {
        if (type == typeof(string))
        {
            return text;
        }

        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying is not null)
        {
            if (text.Length == 0 || IsWhiteSpace(text))
            {
                return null;
            }

            type = underlying;
        }
        else if (!type.IsValueType && text.Length == 0)
        {
            return null;
        }

        return ParseValue(text, type, format, culture);
    }



    /// <summary>
    /// Formats <paramref name="value"/> as field text.
    /// </summary>
    public static string Format(object? value, string? format, CultureInfo culture)
    {
        switch (value)
        {
            case null:
                return string.Empty;
            case string s:
                return s;
            case IFormattable formattable:
                return formattable.ToString(string.IsNullOrEmpty(format) ? null : format, culture);
            default:
                return value.ToString() ?? string.Empty;
        }
    }



    private static object ParseValue(string text, Type type, string? format, CultureInfo culture)
    {
        if (type.IsEnum)
        {
            return ParseEnum(text, type);
        }

        var hasFormat = format is not null && format.Length > 0;
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Boolean => ParseBoolean(text),
            TypeCode.Char => ParseChar(text),
            TypeCode.SByte => sbyte.Parse(text, NumberStyles.Integer, culture),
            TypeCode.Byte => byte.Parse(text, NumberStyles.Integer, culture),
            TypeCode.Int16 => short.Parse(text, NumberStyles.Integer, culture),
            TypeCode.UInt16 => ushort.Parse(text, NumberStyles.Integer, culture),
            TypeCode.Int32 => int.Parse(text, NumberStyles.Integer, culture),
            TypeCode.UInt32 => uint.Parse(text, NumberStyles.Integer, culture),
            TypeCode.Int64 => long.Parse(text, NumberStyles.Integer, culture),
            TypeCode.UInt64 => ulong.Parse(text, NumberStyles.Integer, culture),
            TypeCode.Single => float.Parse(text, NumberStyles.Float | NumberStyles.AllowThousands, culture),
            TypeCode.Double => double.Parse(text, NumberStyles.Float | NumberStyles.AllowThousands, culture),
            TypeCode.Decimal => decimal.Parse(text, NumberStyles.Number, culture),
            TypeCode.DateTime => hasFormat
                ? DateTime.ParseExact(text, format!, culture, DateTimeStyles.None)
                : DateTime.Parse(text, culture, DateTimeStyles.None),
            _ => ParseStruct(text, type, hasFormat ? format : null, culture),
        };
    }



    // The types without a TypeCode of their own.
    private static object ParseStruct(string text, Type type, string? format, CultureInfo culture)
    {
        if (type == typeof(DateTimeOffset))
        {
            return format is not null
                ? DateTimeOffset.ParseExact(text, format, culture, DateTimeStyles.None)
                : DateTimeOffset.Parse(text, culture, DateTimeStyles.None);
        }

        if (type == typeof(TimeSpan))
        {
            return format is not null
                ? TimeSpan.ParseExact(text, format, culture)
                : TimeSpan.Parse(text, culture);
        }

        if (type == typeof(Guid))
        {
            return Guid.Parse(text.Trim());
        }

#if NET6_0_OR_GREATER
        if (type == typeof(DateOnly))
        {
            return format is not null
                ? DateOnly.ParseExact(text, format, culture, DateTimeStyles.None)
                : DateOnly.Parse(text, culture, DateTimeStyles.None);
        }

        if (type == typeof(TimeOnly))
        {
            return format is not null
                ? TimeOnly.ParseExact(text, format, culture, DateTimeStyles.None)
                : TimeOnly.Parse(text, culture, DateTimeStyles.None);
        }
#endif

        if (type == typeof(Uri))
        {
            return new Uri(text, UriKind.RelativeOrAbsolute);
        }

        return ParseWithTypeConverter(text, type, culture);
    }



    private static object ParseEnum(string text, Type type)
    {
        // Names match ignoring case, numbers are accepted, and an undefined name is an error rather
        // than a silent default: CsvHelper 33's EnumConverter rules.
        try
        {
            return Enum.Parse(type, text.Trim(), ignoreCase: true);
        }
        catch (ArgumentException ex)
        {
            throw new FormatException($"'{text}' is not a value of {type.Name}.", ex);
        }
    }



    private static bool ParseBoolean(string text)
    {
        var trimmed = text.Trim();
        if (bool.TryParse(trimmed, out var value))
        {
            return value;
        }

        switch (trimmed)
        {
            case "1":
                return true;
            case "0":
                return false;
            default:
                throw new FormatException($"'{text}' is not a valid boolean.");
        }
    }



    private static char ParseChar(string text)
    {
        if (text.Length == 1)
        {
            return text[0];
        }

        var trimmed = text.Trim();
        if (trimmed.Length == 1)
        {
            return trimmed[0];
        }

        throw new FormatException($"'{text}' is not a single character.");
    }



    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Only reached for member types the converter has no rule for; the record types are reflected under the public constructors' [RequiresUnreferencedCode].")]
    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Only reached for member types the converter has no rule for; the record types are reflected under the public constructors' [RequiresUnreferencedCode].")]
    private static object ParseWithTypeConverter(string text, Type type, CultureInfo culture)
    {
        var converter = TypeDescriptor.GetConverter(type);
        if (!converter.CanConvertFrom(typeof(string)))
        {
            throw new FormatException($"A CSV field cannot be converted to {type.FullName}: the type has no conversion from text.");
        }

        try
        {
            return converter.ConvertFrom(context: null, culture, text)!;
        }
        catch (Exception ex) when (ex is not FormatException)
        {
            throw new FormatException($"'{text}' is not a valid {type.Name}.", ex);
        }
    }



    private static bool IsWhiteSpace(string text)
    {
        foreach (var c in text)
        {
            if (!char.IsWhiteSpace(c))
            {
                return false;
            }
        }

        return true;
    }
}
