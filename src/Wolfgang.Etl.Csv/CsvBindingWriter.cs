using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;
using CsvHelper;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Writes records of one <see cref="CsvRecordBinding"/> to a CsvHelper 34 writer.
/// </summary>
internal static class CsvBindingWriter
{
    /// <summary>Writes the header of <paramref name="binding"/>, without ending the record.</summary>
    public static void WriteHeader(CsvWriter writer, CsvRecordBinding binding)
    {
        foreach (var member in binding.WriteMembers)
        {
            writer.WriteField(member.Name);
        }
    }



    /// <summary>Writes the fields of <paramref name="record"/>, without ending the record.</summary>
    public static void WriteFields
    (
        CsvWriter writer,
        CsvRecordBinding binding,
        object record,
        CultureInfo culture,
        bool trim,
        Func<CsvShouldQuoteContext, bool>? shouldQuote
    )
    {
        foreach (var member in binding.WriteMembers)
        {
            object? value;
            try
            {
                value = member.Property.GetValue(record);
            }
            catch (TargetInvocationException ex) when (ex.InnerException is not null)
            {
                // Surface what the getter threw, not the reflection wrapper.
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }

            var text = CsvValueConverter.Format(value, member.Format, culture);
            if (trim)
            {
                text = text.Trim();
            }

            var quoting = shouldQuote is not null && shouldQuote(new CsvShouldQuoteContext(text, member.Type))
                ? CsvQuoting.Always
                : CsvQuoting.WhenNeededOrWhitespace;

            writer.WriteField(text, quoting);
        }
    }
}
