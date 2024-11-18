using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;

namespace XplorStartupLogger.startuplogger;
// https://github.com/meziantou/Meziantou.Framework/blob/0d8e2c54a36391d1d4b1375249489dcb84d04b72/src/Meziantou.Extensions.Logging.InMemory/InMemoryLoggerProvider.cs
/// <summary>
/// This helper class allows us to access the logging subsystem
/// before runtime logging is fully configured.
/// </summary>
public class StartupLogger :IDisposable
{
    private static ILoggerFactory _startupLoggerFactory;
    private bool disposedValue;
    private static MemoryLoggerProvider memoryLoggerProvider = new MemoryLoggerProvider();

    static StartupLogger()
    {
        var cfg = new ConfigurationBuilder()
                        .AddJsonFile("appSettings.json", optional: false)
                        .Build();
        _startupLoggerFactory = LoggerFactory.Create(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(cfg);
            loggingBuilder.ClearProviders();
            //loggingBuilder.Services.AddSingleton<ILoggerProvider>(memoryLoggerProvider);
        });
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            Console.WriteLine("AppDomain.CurrentDomain.ProcessExit Called");
            if (_startupLoggerFactory != null)
            {
                // any pending logs should be drained to console
                _startupLoggerFactory.Dispose();
                _startupLoggerFactory = null;
            }
        };
        /// register for Cancel.SIGINT/SIGTERM
        Console.CancelKeyPress += (_, ea) =>
        {
            Console.WriteLine("Console.CancelKeyPress Received");
            if (_startupLoggerFactory != null)
            {
                // any pending logs should be drained to console
                _startupLoggerFactory.Dispose();
                _startupLoggerFactory = null;
            }

        };

    }

    public static ILogger CreateLogger(string logName) =>
                _startupLoggerFactory.CreateLogger(logName);

    public static ILogger<T> CreateLogger<T>() =>
            _startupLoggerFactory.CreateLogger<T>();
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                _startupLoggerFactory.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~StartupLogger()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public sealed class MemoryLoggerProvider : ILoggerProvider
{
    // A lock to protect "entries", which may be added to from multiple threads.
    private readonly object _lock = new object();
    private readonly List<LogEntry> entries;
    private readonly IExternalScopeProvider _scopeProvider;

    public MemoryLoggerProvider(IExternalScopeProvider? scopeProvider=default)
    {
        entries = new List<LogEntry>();
        _scopeProvider = scopeProvider  ?? new LoggerExternalScopeProvider();
    }

    public void DrainLogEntries(ILogger logger) 
    {
        lock (_lock)
        {
            foreach (LogEntry entry in entries)
            {
                logger.Log(entry.LogLevel, entry.EventId, entry.State, entry.Exception, entry);
            }
            entries.Clear();
        }
    }

    public IReadOnlyList<LogEntry> GetAllLogEntries()
    {
        lock (_lock)
        {
            return entries.AsReadOnly();
        }
    }
    public void ClearLogEntries()
    {
        lock (_lock)
        {
            entries.Clear();
        }
    }
    public void AddLogEntry(LogEntry log)
    {
        lock (_lock)
        {
            entries.Add(log);
        }
    }

    public ILogger CreateLogger(string categoryName) => new LoggerImpl(this, categoryName,_scopeProvider);

    public void Dispose()
    {
        // Writing entry to console if not already drained.
        lock (_lock)
        {
            foreach (LogEntry entry in entries)
            {
                Console.WriteLine(entry.ToString());
            }
            entries.Clear();
        }
    }

    internal sealed class LoggerImpl : ILogger
    {
        private readonly string? _category;
        private readonly IExternalScopeProvider _scopeProvider;
        private readonly MemoryLoggerProvider _logs;
        public LoggerImpl( MemoryLoggerProvider logs, string category,IExternalScopeProvider scopeProvider )
        {
            _logs = logs;
            _category = category;
            _scopeProvider = scopeProvider;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return _scopeProvider.Push(state);
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var now = DateTimeOffset.UtcNow;
            var scopes = new List<object?>();
            _scopeProvider.ForEachScope((current, scopes) => scopes.Add(current), scopes);
            _logs.AddLogEntry(new LogEntry(now, _category, logLevel, eventId, scopes, state, exception, formatter(state, exception)));
        }
    }


}

public sealed class LogEntry
{
    internal LogEntry(DateTimeOffset createdAt, string? category, LogLevel logLevel, EventId eventId, IReadOnlyList<object?> scopes, object? state, Exception? exception, string message)
    {
        Category = category;
        LogLevel = logLevel;
        EventId = eventId;
        Scopes = scopes;
        State = state;
        Exception = exception;
        Message = message;
        CreatedAt = createdAt;
    }

    public DateTimeOffset CreatedAt { get; }
    public string? Category { get; }
    public LogLevel LogLevel { get; }
    public EventId EventId { get; }
    public IReadOnlyList<object?> Scopes { get; }
    public object? State { get; }
    public Exception? Exception { get; }
    public string Message { get; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (Category is not null)
        {
            sb.Append('[').Append(Category).Append("] ");
        }

        sb.Append(LogLevel);
        if (EventId != default)
        {
            sb.Append(" (");
            sb.Append(EventId.Id);
            sb.Append(' ');
            sb.Append(EventId.Name);
            sb.Append(')');
        }

        sb.Append(": ");
        sb.Append(Message);
        if (Exception is not null)
        {
            sb.Append('\n').Append(Exception);
        }

        if (State is not null)
        {
            sb.Append("\n  => ").Append(JsonSerializer.Serialize(State));
        }

        foreach (var scope in Scopes)
        {
            sb.Append("\n  => ").Append(JsonSerializer.Serialize(scope));
        }

        return sb.ToString();
    }
}
