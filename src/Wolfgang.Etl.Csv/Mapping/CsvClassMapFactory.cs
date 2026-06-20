using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CsvHelper.Configuration;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Builds and caches CsvHelper <see cref="ClassMap{T}"/> instances from
/// <see cref="CsvColumnAttribute"/> and <see cref="CsvIgnoreAttribute"/>
/// decorations on the target type.
/// </summary>
/// <remarks>
/// Caching is per <see cref="Type"/>: the first call for a given <c>T</c>
/// pays the reflection cost, every subsequent call returns the cached map.
/// </remarks>
internal static class CsvClassMapFactory
{
    // The value type is intentionally nullable: a null entry means "this type has
    // no Wolfgang.Etl.Csv attributes, defer to the parser's default conventions".
    // Caching the negative result avoids re-reflecting on every extraction.
    private static readonly ConcurrentDictionary<Type, ClassMap?> Cache = new();

    // Runtime-maps cache: keyed by (TRecord, ColumnMaps reference identity).
    // Two callers passing distinct list instances with identical content still
    // pay reflection once each — by-reference is the only safe key here
    // because CsvColumnMap is mutable (Index/Name/Format/Default are init-only
    // on the type but the list itself can be repopulated). Long-running
    // services that reuse a single ColumnMaps instance across many extractions
    // hit this cache; one-shot callers see no benefit but no regression.
    private static readonly ConcurrentDictionary<RuntimeMapKey, ClassMap> RuntimeMapCache = new();



    private readonly struct RuntimeMapKey : IEquatable<RuntimeMapKey>
    {
        public RuntimeMapKey(Type recordType, IReadOnlyList<CsvColumnMap> columnMaps)
        {
            RecordType = recordType;
            ColumnMaps = columnMaps;
        }

        public Type RecordType { get; }

        public IReadOnlyList<CsvColumnMap> ColumnMaps { get; }

        public bool Equals(RuntimeMapKey other) =>
            RecordType == other.RecordType
            && ReferenceEquals(ColumnMaps, other.ColumnMaps);

        public override bool Equals(object? obj) =>
            obj is RuntimeMapKey other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = RecordType.GetHashCode();
                hash = (hash * 31) + System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(ColumnMaps);
                return hash;
            }
        }
    }



    /// <summary>
    /// Returns a <see cref="ClassMap{T}"/> derived from the
    /// <see cref="CsvColumnAttribute"/> and <see cref="CsvIgnoreAttribute"/>
    /// decorations on <typeparamref name="T"/>, or <c>null</c> when the type
    /// has no Wolfgang.Etl.Csv attributes (in which case the underlying parser
    /// uses its default conventions).
    /// </summary>
    /// <typeparam name="T">The record type being mapped.</typeparam>
    [RequiresUnreferencedCode("Reflects over the public properties of T to build a CsvHelper ClassMap. Not safe under aggressive trimming or NativeAOT without preserving T's properties.")]
    public static ClassMap<T>? GetMap<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>()
    {
        // GetOrAdd makes the get/build/cache sequence atomic so two threads racing
        // for the same T can't both pay the reflection cost or, worse, end up with
        // distinct ClassMap instances if CsvHelper ever became sensitive to identity.
        var cached = Cache.GetOrAdd(typeof(T), static _ => BuildMap<T>());
        return (ClassMap<T>?)cached;
    }



    [RequiresUnreferencedCode("Reflects over the public properties of T to build a CsvHelper ClassMap.")]
    private static ClassMap<T>? BuildMap<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>()
    {
        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Quick-out: if no Wolfgang.Etl.Csv attributes are present, defer to
        // the parser's default behaviour rather than registering a map.
        var hasAnyAttributes = properties.Any
        (
            p => p.IsDefined(typeof(CsvColumnAttribute), inherit: true)
              || p.IsDefined(typeof(CsvIgnoreAttribute), inherit: true)
        );

        if (!hasAnyAttributes)
        {
            return null;
        }

        var map = new DefaultClassMap<T>();
        map.AutoMap(System.Globalization.CultureInfo.CurrentCulture);

        // Index AutoMap's MemberMaps by MemberInfo once so ApplyAttributes can do
        // O(1) lookups instead of an O(N) FirstOrDefault per property — keeping
        // overall map construction O(N) instead of O(N^2) for wide records.
        // MemberMap.Data.Member is typed as MemberInfo? but is always set for entries
        // produced by AutoMap (it's the property/field the map binds to). The
        // null-forgiving operator is safe here.
        var memberMapsByMember = map.MemberMaps.ToDictionary(mm => mm.Data.Member!);

        foreach (var prop in properties)
        {
            ApplyAttributes(map, memberMapsByMember, prop);
        }

        return map;
    }



    /// <summary>
    /// Builds a <see cref="ClassMap{T}"/> from a runtime list of
    /// <see cref="CsvColumnMap"/> descriptors. Used when the layout is selected at
    /// runtime (e.g. from configuration or a database) rather than via attributes.
    /// </summary>
    /// <remarks>
    /// Cached by <c>(typeof(T), columnMaps reference identity)</c>: passing the
    /// same <paramref name="columnMaps"/> instance to subsequent calls returns the
    /// cached <see cref="ClassMap{T}"/> without re-reflecting. Callers running
    /// long-lived services should reuse a single <c>ColumnMaps</c> instance per
    /// template to amortize the reflection cost.
    /// </remarks>
    /// <typeparam name="T">The record type being mapped.</typeparam>
    /// <param name="columnMaps">Runtime column descriptors. Must be non-empty.</param>
    /// <exception cref="ArgumentNullException"><paramref name="columnMaps"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">A descriptor names a property that doesn't exist on <typeparamref name="T"/>.</exception>
    [RequiresUnreferencedCode("Reflects over the public properties of T to build a CsvHelper ClassMap from runtime column descriptors.")]
    public static ClassMap<T> BuildFromColumnMaps<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    (
        IReadOnlyList<CsvColumnMap> columnMaps
    )
    {
        if (columnMaps is null)
        {
            throw new ArgumentNullException(nameof(columnMaps));
        }

        if (columnMaps.Count == 0)
        {
            throw new ArgumentException("columnMaps must contain at least one entry.", nameof(columnMaps));
        }

        ValidateNoDuplicates(columnMaps);

        var key = new RuntimeMapKey(typeof(T), columnMaps);
        if (RuntimeMapCache.TryGetValue(key, out var cached))
        {
            return (ClassMap<T>)cached;
        }

        var built = BuildClassMapFromColumnMaps<T>(columnMaps);
        return (ClassMap<T>)RuntimeMapCache.GetOrAdd(key, built);
    }



    [RequiresUnreferencedCode("Reflects over the public properties of T to build a CsvHelper ClassMap from runtime column descriptors.")]
    private static ClassMap<T> BuildClassMapFromColumnMaps<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    (
        IReadOnlyList<CsvColumnMap> columnMaps
    )
    {
        var type = typeof(T);
        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name, StringComparer.Ordinal);

        var map = new DefaultClassMap<T>();

        foreach (var col in columnMaps)
        {
            ApplyColumnMap(map, properties, type, col);
        }

        return map;
    }



    private static void ApplyColumnMap<T>
    (
        DefaultClassMap<T> map,
        IReadOnlyDictionary<string, PropertyInfo> properties,
        Type type,
        CsvColumnMap col
    )
    {
        if (!properties.TryGetValue(col.PropertyName, out var prop))
        {
            throw new ArgumentException
            (
                $"Property '{col.PropertyName}' was not found on type '{type.FullName}'.",
                nameof(col)
            );
        }

        var memberMap = map.Map(prop.DeclaringType ?? prop.ReflectedType!, prop);

        // Index takes precedence over Name. Per CsvColumnMap.Name's docstring,
        // Name is ignored when Index is non-negative. Apply only one binding
        // path to avoid CsvHelper getting an ambiguous configuration.
        if (col.Index >= 0)
        {
            memberMap.Index(col.Index);
        }
        else if (!string.IsNullOrEmpty(col.Name))
        {
            memberMap.Name(col.Name!);
        }

        if (col.Optional)
        {
            memberMap.Optional();
        }

        if (!string.IsNullOrEmpty(col.Format))
        {
            memberMap.TypeConverterOption.Format(col.Format!);
        }

        if (col.Default is not null)
        {
            memberMap.Default(col.Default);
        }
    }



    private static void ValidateNoDuplicates(IReadOnlyList<CsvColumnMap> columnMaps)
    {
        var seenProperties = new HashSet<string>(StringComparer.Ordinal);
        var seenIndices = new Dictionary<int, string>();

        foreach (var col in columnMaps)
        {
            if (!seenProperties.Add(col.PropertyName))
            {
                throw new ArgumentException
                (
                    $"Duplicate column map for property '{col.PropertyName}'.",
                    nameof(columnMaps)
                );
            }

            if (col.Index >= 0 && seenIndices.TryGetValue(col.Index, out var firstProperty))
            {
                throw new ArgumentException
                (
                    $"Duplicate column index {col.Index} on property '{col.PropertyName}' (already bound by '{firstProperty}').",
                    nameof(columnMaps)
                );
            }

            if (col.Index >= 0)
            {
                seenIndices.Add(col.Index, col.PropertyName);
            }
        }
    }



    private static void ApplyAttributes
    (
        ClassMap map,
        IReadOnlyDictionary<MemberInfo, MemberMap> memberMapsByMember,
        PropertyInfo prop
    )
    {
        if (prop.IsDefined(typeof(CsvIgnoreAttribute), inherit: true))
        {
            if (memberMapsByMember.TryGetValue(prop, out var existing))
            {
                existing.Ignore();
            }
            return;
        }

        var col = prop.GetCustomAttribute<CsvColumnAttribute>(inherit: true);
        if (col is null)
        {
            return;
        }

        var memberMap = memberMapsByMember.TryGetValue(prop, out var existingMap)
            ? existingMap
            : map.Map(prop.DeclaringType ?? prop.ReflectedType!, prop);

        // Index takes precedence over Name (matches CsvColumnAttribute.Name's docstring).
        if (col.Index >= 0)
        {
            memberMap.Index(col.Index);
        }
        else if (!string.IsNullOrEmpty(col.Name))
        {
            memberMap.Name(col.Name!);
        }

        if (col.Optional)
        {
            memberMap.Optional();
        }

        if (!string.IsNullOrEmpty(col.Format))
        {
            memberMap.TypeConverterOption.Format(col.Format!);
        }

        if (col.Default is not null)
        {
            memberMap.Default(col.Default);
        }
    }
}
