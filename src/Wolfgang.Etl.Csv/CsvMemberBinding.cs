using System;
using System.Reflection;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// How one property of a record type maps to a CSV column: the column it reads and writes, and the
/// <c>Optional</c>, <c>Format</c> and <c>Default</c> settings of <see cref="CsvColumnAttribute"/> and
/// <see cref="CsvColumnMap"/>.
/// </summary>
internal sealed class CsvMemberBinding
{
    public CsvMemberBinding(PropertyInfo property, string name, int index, bool optional, string? format, string? defaultValue)
    {
        Property = property;
        Name = name;
        Index = index;
        Optional = optional;
        Format = format is not null && format.Length > 0 ? format : null;
        Default = defaultValue;
    }



    public PropertyInfo Property { get; }



    public Type Type => Property.PropertyType;



    /// <summary>The header name the member reads and writes.</summary>
    public string Name { get; }



    /// <summary>The 0-based column, or -1 when the column is found by <see cref="Name"/>.</summary>
    public int Index { get; }



    public bool Optional { get; }



    public string? Format { get; }



    public string? Default { get; }



    /// <summary>The member's 0-based place in its layout: its column when the file has no header.</summary>
    public int Position { get; internal set; }



    public bool CanRead => Property.GetMethod is { IsPublic: true };



    public bool CanWrite => Property.SetMethod is { IsPublic: true };
}
