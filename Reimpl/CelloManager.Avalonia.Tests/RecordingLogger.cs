#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace CelloManager.Avalonia.Tests;

internal sealed record CapturedLog(LogLevel Level, EventId EventId, string Message, string State, Exception? Exception);

internal sealed class RecordingLogger<T> : ILogger<T>
{
    private readonly ConcurrentQueue<CapturedLog> _events = new();

    public IReadOnlyCollection<CapturedLog> Events => _events.ToArray();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string stateText = state is IEnumerable<KeyValuePair<string, object?>> values
            ? string.Join(";", values.Select(value => $"{value.Key}={value.Value}"))
            : state is null ? string.Empty : state.ToString() ?? string.Empty;
        _events.Enqueue(new CapturedLog(logLevel, eventId, formatter(state, exception), stateText, exception));
    }
}
