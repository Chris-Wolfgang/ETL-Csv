using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace -- the public surface is one flat namespace; Mapping/ is organisational
namespace Wolfgang.Etl.Csv;

/// <summary>
/// A private, immutable copy of a column-map list. The options records and the loader/extractor
/// accept a caller's <see cref="IReadOnlyList{T}"/>, which may really be a <see cref="List{T}"/>
/// the caller can still edit. Taking this copy at the boundary means a later
/// <c>maps[0] = ...</c> on the caller's list cannot change an extractor or loader, and the list the
/// component hands back cannot be cast to something mutable.
/// </summary>
internal sealed class FrozenColumnMaps : IReadOnlyList<CsvColumnMap>
{
    private readonly CsvColumnMap[] _items;



    private FrozenColumnMaps(CsvColumnMap[] items)
    {
        _items = items;
    }



    public int Count => _items.Length;



    public CsvColumnMap this[int index] => _items[index];



    /// <summary>
    /// Returns an immutable snapshot of <paramref name="maps"/>, or the same instance if it is
    /// already one. <see langword="null"/> stays <see langword="null"/>.
    /// </summary>
    internal static IReadOnlyList<CsvColumnMap>? Freeze(IReadOnlyList<CsvColumnMap>? maps)
    {
        if (maps is null || maps is FrozenColumnMaps)
        {
            return maps;
        }

        var copy = new CsvColumnMap[maps.Count];
        for (var i = 0; i < copy.Length; i++)
        {
            copy[i] = maps[i];
        }

        return new FrozenColumnMaps(copy);
    }



    /// <summary>
    /// Freezes every list in <paramref name="maps"/> into a new dictionary, or returns
    /// <see langword="null"/> for <see langword="null"/>.
    /// </summary>
    internal static IReadOnlyDictionary<Type, IReadOnlyList<CsvColumnMap>>? Freeze
    (
        IReadOnlyDictionary<Type, IReadOnlyList<CsvColumnMap>>? maps
    )
    {
        if (maps is null)
        {
            return null;
        }

        var copy = new Dictionary<Type, IReadOnlyList<CsvColumnMap>>(maps.Count);
        foreach (var pair in maps)
        {
            copy[pair.Key] = Freeze(pair.Value)!;
        }

        return copy;
    }



    public IEnumerator<CsvColumnMap> GetEnumerator() => ((IEnumerable<CsvColumnMap>)_items).GetEnumerator();



    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
