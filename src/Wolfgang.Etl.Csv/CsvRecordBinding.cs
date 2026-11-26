using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// The column layout of a record type, built once by reflection from its public properties, its
/// <see cref="CsvColumnAttribute"/> / <see cref="CsvIgnoreAttribute"/> attributes, or a list of
/// <see cref="CsvColumnMap"/>s. It replaces the CsvHelper 33 <c>ClassMap</c> the library used to
/// register: CsvHelper 34 maps only source-generated or hand-written record types, so the library
/// reads and writes its consumers' plain record types field by field through this layout.
/// </summary>
internal sealed class CsvRecordBinding
{
    private readonly ConstructorInfo? _defaultConstructor;
    private readonly ConstructorInfo? _constructor;
    private readonly CsvMemberBinding?[] _constructorMembers;
    private readonly ParameterInfo[] _constructorParameters;



    // The constructors are found by reflection too; the record types reach here only under the public
    // CsvExtractor / CsvLoader constructors' [RequiresUnreferencedCode].
    [RequiresUnreferencedCode("Reflects over the public constructors of the record type.")]
    private CsvRecordBinding(Type recordType, IReadOnlyList<CsvMemberBinding> members)
    {
        RecordType = recordType;
        Members = members;

        for (var i = 0; i < members.Count; i++)
        {
            members[i].Position = i;
        }

        _defaultConstructor = recordType.IsValueType ? null : recordType.GetConstructor(Type.EmptyTypes);
        _constructorParameters = Array.Empty<ParameterInfo>();
        _constructorMembers = Array.Empty<CsvMemberBinding?>();

        if (!recordType.IsValueType && _defaultConstructor is null)
        {
            // No parameterless constructor (a positional record, an immutable class): bind the
            // public constructor with the most parameters, each parameter to the member of the same
            // name, as CsvHelper 33 did.
            _constructor = recordType
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            if (_constructor is not null)
            {
                _constructorParameters = _constructor.GetParameters();
                _constructorMembers = _constructorParameters
                    .Select(p => members.FirstOrDefault(m => string.Equals(m.Property.Name, p.Name, StringComparison.OrdinalIgnoreCase)))
                    .ToArray();
            }
        }

        ReadMembers = members
            .Where(m => m.CanWrite && Array.IndexOf(_constructorMembers, m) < 0)
            .ToArray();
        // Written in column order: an explicit Index places a member, the others keep their place.
        WriteMembers = members
            .Where(m => m.CanRead)
            .OrderBy(m => m.Index >= 0 ? m.Index : m.Position)
            .ToArray();
    }



    public Type RecordType { get; }



    /// <summary>Every mapped member, in column order.</summary>
    public IReadOnlyList<CsvMemberBinding> Members { get; }



    /// <summary>The members set after construction when a record is read.</summary>
    public IReadOnlyList<CsvMemberBinding> ReadMembers { get; }



    /// <summary>The members written, in column order.</summary>
    public IReadOnlyList<CsvMemberBinding> WriteMembers { get; }



    /// <summary>The constructor parameters a read binds, each to its member (or <see langword="null"/>).</summary>
    public IReadOnlyList<CsvMemberBinding?> ConstructorMembers => _constructorMembers;



    public IReadOnlyList<ParameterInfo> ConstructorParameters => _constructorParameters;



    /// <summary>
    /// The layout of <paramref name="recordType"/> by its public properties and attributes.
    /// </summary>
    [RequiresUnreferencedCode("Reflects over the public properties and constructors of the record type.")]
    public static CsvRecordBinding FromAttributes
    (
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type recordType
    )
    {
        var members = new List<CsvMemberBinding>();
        foreach (var property in GetProperties(recordType))
        {
            // Attribute.IsDefined walks overridden properties; PropertyInfo.IsDefined ignores 'inherit'.
            if (Attribute.IsDefined(property, typeof(CsvIgnoreAttribute), inherit: true))
            {
                continue;
            }

            var column = property.GetCustomAttribute<CsvColumnAttribute>(inherit: true);
            members.Add
            (
                column is null
                    ? new CsvMemberBinding(property, property.Name, -1, optional: false, format: null, defaultValue: null)
                    : Create(property, column.Index, column.Name, column.Optional, column.Format, column.Default)
            );
        }

        return new CsvRecordBinding(recordType, members);
    }



    /// <summary>
    /// The layout of <paramref name="recordType"/> given by <paramref name="columnMaps"/>: only the
    /// listed properties, in list order.
    /// </summary>
    /// <exception cref="ArgumentException">A column map names a property the type does not have.</exception>
    [RequiresUnreferencedCode("Reflects over the public properties and constructors of the record type.")]
    public static CsvRecordBinding FromColumnMaps
    (
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type recordType,
        IReadOnlyList<CsvColumnMap> columnMaps
    )
    {
        var properties = GetProperties(recordType).ToDictionary(p => p.Name, StringComparer.Ordinal);
        var members = new List<CsvMemberBinding>(columnMaps.Count);

        foreach (var column in columnMaps)
        {
            if (!properties.TryGetValue(column.PropertyName, out var property))
            {
                throw new ArgumentException
                (
                    $"Property '{column.PropertyName}' was not found on type '{recordType.FullName}'.",
                    nameof(columnMaps)
                );
            }

            members.Add(Create(property, column.Index, column.Name, column.Optional, column.Format, column.Default));
        }

        return new CsvRecordBinding(recordType, members);
    }



    /// <summary>Creates an instance from the constructor arguments, or the parameterless constructor.</summary>
    public object CreateInstance(object?[] constructorArguments)
    {
        if (_constructor is not null)
        {
            return _constructor.Invoke(constructorArguments);
        }

        if (_defaultConstructor is not null)
        {
            return _defaultConstructor.Invoke(parameters: null);
        }

        // A struct, or a class without a public constructor: Activator handles the struct case and
        // reports the class case.
        return Activator.CreateInstance(RecordType)!;
    }



    // Index takes precedence over Name (per both CsvColumnAttribute's and CsvColumnMap's docs).
    private static CsvMemberBinding Create(PropertyInfo property, int index, string? name, bool optional, string? format, string? defaultValue)
    {
        var columnName = index < 0 && name is not null && name.Length > 0 ? name : property.Name;
        return new CsvMemberBinding(property, columnName, index < 0 ? -1 : index, optional, format, defaultValue);
    }



    // Public instance properties in Type.GetProperties order (the derived type's own properties first,
    // then its base type's), as CsvHelper 33's AutoMap numbered them; indexers are skipped, and a hidden
    // base property is represented by the most derived declaration.
    private static IEnumerable<PropertyInfo> GetProperties
    (
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type
    )
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.GetIndexParameters().Length == 0 && seen.Add(property.Name))
            {
                yield return property;
            }
        }
    }
}
