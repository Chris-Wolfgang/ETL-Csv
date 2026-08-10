using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Describes how a single CSV file mixes multiple record shapes, keyed by a discriminator column, so a
/// <see cref="CsvExtractor{TRecord}"/> binds each row to the right concrete type and a
/// <see cref="CsvLoader{TRecord}"/> writes each record's type and columns. Build one with
/// <see cref="CsvDiscriminatorBuilder{TBase}"/> — that path is trim/AOT-safe. Constructing it directly
/// from a <see cref="Mapping"/> dictionary is supported but reflects over the mapped types at run time.
/// </summary>
/// <typeparam name="TBase">The common base type / interface the concrete row types derive from.</typeparam>
public sealed class CsvDiscriminator<TBase>
    where TBase : notnull
{
    private static readonly IReadOnlyDictionary<string, Type> EmptyMapping = new Dictionary<string, Type>();

    private Dictionary<string, Type>? _typeByValue;
    private Dictionary<Type, string>? _valueByType;


    /// <summary>The 0-based index of the discriminator column. Ignored when <see cref="ColumnName"/> is set.</summary>
    public int ColumnIndex { get; init; }


    /// <summary>The header name of the discriminator column. When set, it takes precedence over <see cref="ColumnIndex"/>.</summary>
    public string? ColumnName { get; init; }


    /// <summary>Maps each discriminator value to the concrete record type. Every value type must derive from / implement <typeparamref name="TBase"/>.</summary>
    public IReadOnlyDictionary<string, Type> Mapping { get; init; } = EmptyMapping;


    /// <summary>How to handle a discriminator value (read) or record type (write) that isn't in <see cref="Mapping"/>. Defaults to <see cref="CsvDiscriminatorAction.Throw"/>.</summary>
    public CsvDiscriminatorAction UnknownDiscriminator { get; init; } = CsvDiscriminatorAction.Throw;


    /// <summary>Optional per-type runtime column maps, applied instead of the concrete type's attributes.</summary>
    public IReadOnlyDictionary<Type, IReadOnlyList<CsvColumnMap>>? PerTypeColumnMaps { get; init; }


    /// <summary>The comparer used to match discriminator values. Defaults to <see cref="StringComparer.OrdinalIgnoreCase"/>.</summary>
    public StringComparer Comparer { get; init; } = StringComparer.OrdinalIgnoreCase;


    // Per-type CsvHelper ClassMaps built AOT-safely by the builder. Null for the direct init-form,
    // in which case RegisterClassMaps builds them reflectively.
    internal IReadOnlyList<ClassMap>? PrebuiltClassMaps { get; init; }


    internal bool TryResolveType(string value, out Type type)
    {
        _typeByValue ??= BuildTypeByValue();
        return _typeByValue.TryGetValue(value, out type!);
    }


    internal bool TryResolveValue(Type type, out string value)
    {
        _valueByType ??= BuildValueByType();
        return _valueByType.TryGetValue(type, out value!);
    }


    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Builder path pre-builds maps AOT-safely; the reflective fallback is only reached from the [RequiresUnreferencedCode] direct init-form.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Builder path pre-builds maps AOT-safely; the reflective fallback is only reached from the [RequiresUnreferencedCode] direct init-form.")]
    internal void RegisterClassMaps(CsvContext context)
    {
        if (PrebuiltClassMaps is not null)
        {
            foreach (var map in PrebuiltClassMaps)
            {
                context.RegisterClassMap(map);
            }

            return;
        }

        foreach (var type in Mapping.Values.Distinct())
        {
            var columns = PerTypeColumnMaps is not null && PerTypeColumnMaps.TryGetValue(type, out var maps)
                ? maps
                : null;

            var map = columns is { Count: > 0 }
                ? CsvClassMapFactory.BuildFromColumnMaps(type, columns)
                : CsvClassMapFactory.GetMap(type);

            if (map is not null)
            {
                context.RegisterClassMap(map);
            }
        }
    }


    private Dictionary<string, Type> BuildTypeByValue()
    {
        var lookup = new Dictionary<string, Type>(Comparer);
        foreach (var pair in Mapping)
        {
            lookup[pair.Key] = pair.Value;
        }

        return lookup;
    }


    private Dictionary<Type, string> BuildValueByType()
    {
        var lookup = new Dictionary<Type, string>();
        foreach (var pair in Mapping)
        {
            lookup[pair.Value] = pair.Key;
        }

        return lookup;
    }
}
