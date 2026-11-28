using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace -- the public surface is one flat namespace; Mapping/ is organisational
namespace Wolfgang.Etl.Csv;

/// <summary>
/// Fluent, trim/AOT-safe builder for a <see cref="CsvDiscriminator{TBase}"/>. Each
/// <see cref="Map{T}(string, IReadOnlyList{CsvColumnMap})"/> call names a concrete type generically,
/// so the mapping and its per-type <see cref="CsvColumnMap"/>s are resolved with the property metadata
/// trimming/AOT needs preserved.
/// </summary>
/// <typeparam name="TBase">The common base type / interface the concrete row types derive from.</typeparam>
public sealed class CsvDiscriminatorBuilder<TBase>
    where TBase : notnull
{
    private readonly int _columnIndex;
    private readonly string? _columnName;
    private readonly List<Entry> _entries = new();
    private CsvDiscriminatorAction _onUnknown = CsvDiscriminatorAction.Throw;
    private StringComparer _comparer = StringComparer.OrdinalIgnoreCase;


    /// <summary>Creates a builder that reads the discriminator from a 0-based column index.</summary>
    /// <param name="columnIndex">The 0-based discriminator column index.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="columnIndex"/> is negative.</exception>
    public CsvDiscriminatorBuilder(int columnIndex)
    {
        if (columnIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, "Column index must be non-negative.");
        }

        _columnIndex = columnIndex;
    }


    /// <summary>Creates a builder that reads the discriminator from a named column (requires a header row).</summary>
    /// <param name="columnName">The discriminator column's header name.</param>
    /// <exception cref="ArgumentException"><paramref name="columnName"/> is null, empty, or whitespace.</exception>
    public CsvDiscriminatorBuilder(string columnName)
    {
        _columnName = string.IsNullOrWhiteSpace(columnName)
            ? throw new ArgumentException("Column name must not be empty or whitespace.", nameof(columnName))
            : columnName;
        _columnIndex = -1;
    }


    /// <summary>Maps a discriminator value to a concrete record type.</summary>
    /// <typeparam name="T">The concrete type, deriving from / implementing <typeparamref name="TBase"/>.</typeparam>
    /// <param name="discriminatorValue">The value in the discriminator column that selects <typeparamref name="T"/>.</param>
    /// <param name="columnMaps">Optional per-type runtime column maps; when omitted, <typeparamref name="T"/>'s attributes apply.</param>
    /// <returns>The same builder, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="discriminatorValue"/> is null.</exception>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "T is annotated with PublicProperties; the binding reflects over T's public properties, and over its constructors only under the CsvExtractor / CsvLoader constructors' [RequiresUnreferencedCode].")]
    public CsvDiscriminatorBuilder<TBase> Map<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    (
        string discriminatorValue,
        IReadOnlyList<CsvColumnMap>? columnMaps = null
    )
        where T : TBase
    {
        if (discriminatorValue is null)
        {
            throw new ArgumentNullException(nameof(discriminatorValue));
        }

        // Built here, where T is a compile-time type whose properties the trimmer keeps, so neither the
        // extractor nor the loader reflects over a runtime-only type for a builder-made discriminator.
        var binding = CsvRecordBindingFactory.Resolve(typeof(T), columnMaps);

        _entries.Add(new Entry(discriminatorValue, typeof(T), binding, columnMaps));

        return this;
    }


    /// <summary>Sets how an unmapped discriminator value / record type is handled. Defaults to <see cref="CsvDiscriminatorAction.Throw"/>.</summary>
    /// <param name="action">The action to take for an unmapped value or type.</param>
    /// <returns>This builder, for chaining.</returns>
    public CsvDiscriminatorBuilder<TBase> OnUnknown(CsvDiscriminatorAction action)
    {
        _onUnknown = action;
        return this;
    }


    /// <summary>Sets the comparer used to match discriminator values. Defaults to <see cref="StringComparer.OrdinalIgnoreCase"/>.</summary>
    /// <param name="comparer">The comparer for discriminator values.</param>
    /// <returns>This builder, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
    public CsvDiscriminatorBuilder<TBase> WithComparer(StringComparer comparer)
    {
        _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        return this;
    }


    /// <summary>Builds the immutable <see cref="CsvDiscriminator{TBase}"/>.</summary>
    /// <returns>The configured discriminator.</returns>
    /// <exception cref="InvalidOperationException">
    /// The same discriminator value, or the same concrete type, was mapped more than once.
    /// </exception>
    public CsvDiscriminator<TBase> Build()
    {
        var mapping = new Dictionary<string, Type>(_comparer);
        var perTypeColumnMaps = new Dictionary<Type, IReadOnlyList<CsvColumnMap>>();
        var bindings = new Dictionary<Type, CsvRecordBinding>();
        var seenTypes = new HashSet<Type>();

        foreach (var entry in _entries)
        {
            // Reject ambiguous mappings up front: a value mapped twice, or a type mapped to two values
            // (which would make the loader's reverse type -> value lookup non-deterministic).
            if (mapping.ContainsKey(entry.Value))
            {
                throw new InvalidOperationException($"Discriminator value '{entry.Value}' is mapped more than once.");
            }

            if (!seenTypes.Add(entry.Type))
            {
                throw new InvalidOperationException($"Type '{entry.Type}' is mapped to more than one discriminator value.");
            }

            mapping[entry.Value] = entry.Type;
            bindings[entry.Type] = entry.Binding;

            if (entry.Columns is { Count: > 0 })
            {
                perTypeColumnMaps[entry.Type] = entry.Columns;
            }
        }

        return new CsvDiscriminator<TBase>
        {
            ColumnIndex = _columnName is null ? _columnIndex : 0,
            ColumnName = _columnName,
            Mapping = mapping,
            UnknownDiscriminator = _onUnknown,
            Comparer = _comparer,
            PerTypeColumnMaps = perTypeColumnMaps.Count > 0 ? perTypeColumnMaps : null,
            PrebuiltBindings = bindings,
        };
    }


    private sealed record Entry(string Value, Type Type, CsvRecordBinding Binding, IReadOnlyList<CsvColumnMap>? Columns);
}
