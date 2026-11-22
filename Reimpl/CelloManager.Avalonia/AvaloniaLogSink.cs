using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Avalonia.Logging;
using Microsoft.Extensions.Logging;

namespace CelloManager;

internal sealed class AvaloniaLogSink(ILoggerFactory loggerFactory) : ILogSink
{
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new(StringComparer.Ordinal);

    private ILogger GetLogger(string area) => _loggers.GetOrAdd(area, static (key, lf) => lf.CreateLogger(key), loggerFactory);

    public bool IsEnabled(LogEventLevel level, string area)
    {
        return level switch
        {
            LogEventLevel.Verbose => false,
            LogEventLevel.Debug => false,
            LogEventLevel.Information => false,
            LogEventLevel.Warning => true,
            LogEventLevel.Error => true,
            LogEventLevel.Fatal => true,
            _ => false,
        };
        
        //return GetLogger(area).IsEnabled(ToLogLevel(level));
    }

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
    {
        #pragma warning disable CA2254
        // ReSharper disable TemplateIsNotCompileTimeConstantProblem
        #pragma warning disable CA1873
        GetLogger(area).Log(ToLogLevel(level), messageTemplate, source);
    }

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues) 
        => GetLogger(area).Log(ToLogLevel(level), messageTemplate, ImmutableArray<object?>.Empty.Add(source).AddRange(propertyValues));

    private static LogLevel ToLogLevel(LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Verbose => LogLevel.Trace,
            LogEventLevel.Debug => LogLevel.Debug,
            LogEventLevel.Information => LogLevel.Information,
            LogEventLevel.Warning => LogLevel.Warning,
            LogEventLevel.Error => LogLevel.Error,
            LogEventLevel.Fatal => LogLevel.Critical,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, message: null),
        };
    }
}