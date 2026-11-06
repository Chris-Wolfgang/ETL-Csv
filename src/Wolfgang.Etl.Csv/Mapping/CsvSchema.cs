using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// A record type's CSV column mapping as a first-class, introspectable object — the code-first
/// counterpart of <see cref="CsvColumnAttribute"/> / <see cref="CsvIgnoreAttribute"/> decorations.
/// Produced by <see cref="CsvSchemaBuilder{T}.Build"/>, or reified from a type's attributes by
/// <see cref="For{T}"/>, and applied through <see cref="CsvExtractorOptions{TRecord}.Schema"/> /
/// <see cref="CsvLoaderOptions{TRecord}.Schema"/>.
/// </summary>
/// <remarks>
/// <para>
/// A schema is bound to the <see cref="RecordType"/> it was built for: an extractor or loader for a
/// different record type rejects it at construction. It is applied through the same internal
/// column-map path as <c>ColumnMaps</c> (with which it is mutually exclusive), so a code-built
/// schema behaves identically to the equivalent attributed record.
/// </para>
/// <para>
/// <see cref="Columns"/> is read-only and is the identity key for the compiled map's cache, so
/// reuse one <see cref="CsvSchema"/> instance across runs rather than rebuilding it per call.
/// </para>
/// </remarks>
public sealed class CsvSchema
{
    internal CsvSchema(Type recordType, IReadOnlyList<CsvColumnMap> columns)
    {
        RecordType = recordType ?? throw new ArgumentNullException(nameof(recordType));

        if (columns is null)
        {
            throw new ArgumentNullException(nameof(columns));
        }

        if (columns.Count == 0)
        {
            throw new ArgumentException("A schema must map at least one column.", nameof(columns));
        }

        Columns = columns;
    }



    /// <summary>Gets the record type this schema maps.</summary>
    public Type RecordType { get; }



    /// <summary>Gets the column mappings, in the order they were defined. The list is read-only.</summary>
    public IReadOnlyList<CsvColumnMap> Columns { get; }



    /// <summary>Gets the number of mapped columns.</summary>
    public int ColumnCount => Columns.Count;



    /// <summary>
    /// Reifies the attribute-resolved mapping of <typeparamref name="T"/> as a schema: every public
    /// instance property becomes a column, honouring <see cref="CsvColumnAttribute"/> and omitting
    /// members marked <see cref="CsvIgnoreAttribute"/>. Undecorated properties bind by their own
    /// name, exactly as the attribute path does.
    /// </summary>
    /// <typeparam name="T">The record type to introspect.</typeparam>
    /// <returns>The type's mapping as a <see cref="CsvSchema"/>.</returns>
    /// <exception cref="InvalidOperationException"><typeparamref name="T"/> has no mappable public properties.</exception>
    [RequiresUnreferencedCode("Reflects over the public properties of T to build the schema.")]
    public static CsvSchema For<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>()
    {
        var columns = new List<CsvColumnMap>();

        foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.IsDefined(typeof(CsvIgnoreAttribute), inherit: true))
            {
                continue;
            }

            var attribute = property.GetCustomAttribute<CsvColumnAttribute>(inherit: true);

            columns.Add
            (
                attribute is null
                    ? new CsvColumnMap(property.Name) { Name = property.Name }
                    : new CsvColumnMap(property.Name)
                    {
                        Name = attribute.Name,
                        Index = attribute.Index,
                        Optional = attribute.Optional,
                        Format = attribute.Format,
                        Default = attribute.Default,
                    }
            );
        }

        if (columns.Count == 0)
        {
            throw new InvalidOperationException($"{typeof(T).Name} has no mappable public properties.");
        }

        return new CsvSchema(typeof(T), Array.AsReadOnly(columns.ToArray()));
    }



    // Shared fail-fast rule for the extractor/loader options: a schema is mutually exclusive with
    // ColumnMaps and must have been built for the component's record type.
    internal static void ValidateOptions
    (
        CsvSchema? schema,
        IReadOnlyList<CsvColumnMap>? columnMaps,
        Type recordType,
        string paramName
    )
    {
        if (schema is null)
        {
            return;
        }

        if (columnMaps is { Count: > 0 })
        {
            throw new ArgumentException("Set either Schema or ColumnMaps, not both.", paramName);
        }

        if (schema.RecordType != recordType)
        {
            throw new ArgumentException
            (
                $"Schema was built for '{schema.RecordType.Name}' but this component is for '{recordType.Name}'.",
                paramName
            );
        }
    }
}
