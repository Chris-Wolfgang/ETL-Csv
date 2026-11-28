using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;
using CsvHelper;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Reads records of one <see cref="CsvRecordBinding"/> from a CsvHelper 34 reader: the columns are
/// resolved once against the header, then each record is built field by field.
/// </summary>
internal sealed class CsvBindingReader
{
    private readonly CsvRecordBinding _binding;
    private readonly int[] _readColumns;
    private readonly int[] _constructorColumns;
    private readonly bool _tolerateMissingFields;
    private readonly CultureInfo _culture;
    private readonly IReadOnlyList<string>? _headers;



    /// <param name="binding">The layout to read.</param>
    /// <param name="reader">The reader, positioned after its header (when it has one).</param>
    /// <param name="hasHeader">Whether columns are found by header name.</param>
    /// <param name="validateHeader">
    /// Whether a required column missing from the header is an error. Off for a polymorphic file, whose
    /// header describes the discriminator column and each record type, and for input without a header row.
    /// </param>
    /// <param name="tolerateMissingFields">
    /// Whether a field missing from a record leaves its member unset instead of failing the record. On
    /// for a polymorphic file, whose records are shaped per record type.
    /// </param>
    /// <param name="culture">The culture fields are parsed with.</param>
    /// <exception cref="CsvFormatException">A required column is missing from the header.</exception>
    public CsvBindingReader
    (
        CsvRecordBinding binding,
        CsvReader reader,
        bool hasHeader,
        bool validateHeader,
        bool tolerateMissingFields,
        CultureInfo culture
    )
    {
        _binding = binding;
        _tolerateMissingFields = tolerateMissingFields;
        _culture = culture;
        _headers = hasHeader ? reader.Headers : null;

        var missing = validateHeader ? new List<string>() : null;

        _readColumns = new int[binding.ReadMembers.Count];
        for (var i = 0; i < _readColumns.Length; i++)
        {
            _readColumns[i] = Resolve(binding.ReadMembers[i], reader, hasHeader, missing);
        }

        _constructorColumns = new int[binding.ConstructorMembers.Count];
        for (var i = 0; i < _constructorColumns.Length; i++)
        {
            var member = binding.ConstructorMembers[i];
            _constructorColumns[i] = member is null ? -1 : Resolve(member, reader, hasHeader, missing);
        }

        if (missing is { Count: > 0 })
        {
            throw new CsvFormatException
            (
                $"The header has no column named '{string.Join("', '", missing)}', which {binding.RecordType.Name} requires. "
                + "Mark the member Optional, or map it to a column the file has."
            );
        }
    }



    /// <summary>
    /// The 0-based column of the field the last failed <see cref="Read"/> could not read (missing, or
    /// not convertible), or -1 when the failure was not a field's.
    /// </summary>
    public int FailedColumn { get; private set; } = -1;



    /// <summary>The header name of <see cref="FailedColumn"/>, or <see langword="null"/>.</summary>
    public string? FailedColumnName { get; private set; }



    /// <summary>The text of the field at <see cref="FailedColumn"/>, or <see langword="null"/>.</summary>
    public string? FailedValue { get; private set; }



    /// <summary>Reads the reader's current record.</summary>
    /// <exception cref="CsvFormatException">A field is missing or cannot be converted to its member's type.</exception>
    public object Read(CsvReader reader)
    {
        FailedColumn = -1;
        FailedColumnName = null;
        FailedValue = null;

        var arguments = new object?[_constructorColumns.Length];
        for (var i = 0; i < arguments.Length; i++)
        {
            var member = _binding.ConstructorMembers[i];
            var parameter = _binding.ConstructorParameters[i];
            arguments[i] = member is not null && TryReadValue(reader, member, _constructorColumns[i], out var value)
                ? value
                : DefaultArgument(parameter);
        }

        var record = _binding.CreateInstance(arguments);

        for (var i = 0; i < _readColumns.Length; i++)
        {
            var member = _binding.ReadMembers[i];
            if (TryReadValue(reader, member, _readColumns[i], out var value))
            {
                member.Property.SetValue(record, value);
            }
        }

        return record;
    }



    private static int Resolve(CsvMemberBinding member, CsvReader reader, bool hasHeader, List<string>? missing)
    {
        if (member.Index >= 0)
        {
            return member.Index;
        }

        if (!hasHeader)
        {
            // Without a header the columns are the members in order, as CsvHelper 33's AutoMap numbered them.
            return member.Position;
        }

        if (reader.TryGetFieldIndex(member.Name, out var index))
        {
            return index;
        }

        if (!member.Optional)
        {
            missing?.Add(member.Name);
        }

        return -1;
    }



    // Whether the record has a value for the member: its field, or its Default when the field is
    // empty or the column is absent. False leaves the member as the type initialises it.
    private bool TryReadValue(CsvReader reader, CsvMemberBinding member, int column, out object? value)
    {
        string? text;
        if (column < 0)
        {
            text = member.Default;
        }
        else if (column >= reader.FieldCount)
        {
            if (!_tolerateMissingFields)
            {
                FailedColumn = column;
                FailedColumnName = HeaderName(column);
                throw new CsvFormatException
                (
                    $"Record {reader.RowNumber} has no field {column + 1} for {_binding.RecordType.Name}.{member.Property.Name}.",
                    reader.RowNumber,
                    reader.ByteOffset
                );
            }

            text = member.Default;
        }
        else
        {
            text = reader.GetString(column);
            if (text.Length == 0 && member.Default is not null)
            {
                text = member.Default;
            }
        }

        if (text is null)
        {
            value = null;
            return false;
        }

        try
        {
            value = CsvValueConverter.Parse(text, member.Type, member.Format, _culture);
            return true;
        }
        catch (Exception ex) when (ex is FormatException or OverflowException or ArgumentException or NotSupportedException)
        {
            FailedColumn = column;
            FailedColumnName = column >= 0 ? HeaderName(column) : null;
            FailedValue = text;
            throw new CsvFormatException
            (
                $"Record {reader.RowNumber}: the field '{text}' of column '{member.Name}' cannot be read as the {member.Type.Name} of {_binding.RecordType.Name}.{member.Property.Name}.",
                reader.RowNumber,
                reader.ByteOffset,
                ex
            );
        }
    }



    // The header name of a column, or null without a header or past its end.
    private string? HeaderName(int column) =>
        _headers is not null && column < _headers.Count ? _headers[column] : null;



    private static object? DefaultArgument(ParameterInfo parameter)
    {
        if (parameter.HasDefaultValue)
        {
            return parameter.DefaultValue;
        }

        return parameter.ParameterType.IsValueType ? CsvRecordBinding.DefaultValue(parameter.ParameterType) : null;
    }
}
