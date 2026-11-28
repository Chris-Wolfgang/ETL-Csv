using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Builds and caches the <see cref="CsvRecordBinding"/> of a record type, from its attributes or from
/// a list of <see cref="CsvColumnMap"/>s.
/// </summary>
internal static class CsvRecordBindingFactory
{
    private static readonly ConcurrentDictionary<Type, CsvRecordBinding> Cache = new();

    // Keyed weakly by the ColumnMaps list instance, so an entry dies with the list (no unbounded growth
    // when a caller builds a fresh list per run). Each entry keeps a snapshot of the list's contents at
    // build time and is revalidated on every hit, so a list that is repopulated for a different layout
    // is rebuilt rather than served a stale binding.
    private static readonly ConditionalWeakTable<IReadOnlyList<CsvColumnMap>, ConcurrentDictionary<Type, ColumnMapsEntry>> ColumnMapsCache = new();



    private sealed class ColumnMapsEntry
    {
        public ColumnMapsEntry(CsvColumnMap[] snapshot, CsvRecordBinding binding)
        {
            Snapshot = snapshot;
            Binding = binding;
        }



        public CsvColumnMap[] Snapshot { get; }



        public CsvRecordBinding Binding { get; }
    }



    /// <summary>
    /// The layout of <paramref name="type"/> by its public properties and its
    /// <see cref="CsvColumnAttribute"/> / <see cref="CsvIgnoreAttribute"/> attributes.
    /// </summary>
    [RequiresUnreferencedCode("Reflects over the public properties and constructors of the record type.")]
    public static CsvRecordBinding GetBinding
    (
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type
    ) =>
        Cache.GetOrAdd(type, CsvRecordBinding.FromAttributes);



    /// <summary>
    /// The layout of <paramref name="type"/> given by <paramref name="columnMaps"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="columnMaps"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="columnMaps"/> is empty, maps a property twice or a column index twice, or names a
    /// property the type does not have.
    /// </exception>
    [RequiresUnreferencedCode("Reflects over the public properties and constructors of the record type.")]
    public static CsvRecordBinding FromColumnMaps
    (
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type,
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

        var byType = ColumnMapsCache.GetValue(columnMaps, _ => new ConcurrentDictionary<Type, ColumnMapsEntry>());
        var current = columnMaps.ToArray();

        if (byType.TryGetValue(type, out var cached) && cached.Snapshot.SequenceEqual(current))
        {
            return cached.Binding;
        }

        var built = CsvRecordBinding.FromColumnMaps(type, columnMaps);
        byType[type] = new ColumnMapsEntry(current, built);
        return built;
    }



    /// <summary>
    /// The layout of <paramref name="type"/>: from <paramref name="columnMaps"/> when it has entries,
    /// else from the type's attributes.
    /// </summary>
    [RequiresUnreferencedCode("Reflects over the public properties and constructors of the record type.")]
    public static CsvRecordBinding Resolve
    (
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type,
        IReadOnlyList<CsvColumnMap>? columnMaps
    ) =>
        columnMaps is { Count: > 0 }
            ? FromColumnMaps(type, columnMaps)
            : GetBinding(type);



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
}
