using System;

namespace Wolfgang.Etl.Csv.Tests.Unit.TestModels;

/// <summary>
/// A synchronous <see cref="IProgress{T}"/> capture, useful in tests where the
/// async dispatch of <see cref="Progress{T}"/> would race with assertions.
/// </summary>
internal sealed class SyncProgress<T> : IProgress<T>
{
    public T? LastValue { get; private set; }

    public int CallCount { get; private set; }

    public void Report(T value)
    {
        LastValue = value;
        CallCount++;
    }
}
