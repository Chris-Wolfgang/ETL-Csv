using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wolfgang.Etl.Csv.Benchmarks;

/// <summary>Shared helpers for the benchmark classes.</summary>
internal static class BenchmarkAsync
{
    /// <summary>Exposes an in-memory sequence as an <see cref="IAsyncEnumerable{T}"/> for the loaders.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The items to yield.</param>
    /// <returns>An async sequence over <paramref name="source"/>.</returns>
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            yield return item;
        }

        await Task.CompletedTask;
    }
}
