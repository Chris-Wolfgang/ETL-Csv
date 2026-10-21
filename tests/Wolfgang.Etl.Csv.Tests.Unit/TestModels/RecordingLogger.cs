using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Etl.Csv.Tests.Unit.TestModels;

/// <summary>
/// An <see cref="ILogger{TCategoryName}"/> that records every entry, so tests can assert what a
/// stage logged (event id and rendered message).
/// </summary>
internal sealed class RecordingLogger<T> : ILogger<T>
{
    public List<(LogLevel Level, EventId EventId, string Message)> Entries { get; } = new();



    // Required by ILogger; the stages under test never open a scope.
    [ExcludeFromCodeCoverage]
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => null;



    public bool IsEnabled(LogLevel logLevel) => true;



    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
        Entries.Add((logLevel, eventId, formatter(state, exception)));
}
