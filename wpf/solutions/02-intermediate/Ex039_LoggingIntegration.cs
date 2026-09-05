// Exercise 039 - Logging integration (intermediate).
// REFERENCE SOLUTION.
// Goal:   Give a view model a real, host-provided ILogger<T> instead of Console.WriteLine
//         or nothing at all, and use a scope to tag every log line one operation produces
//         with context that stays attached for as long as that operation runs.
// Drills: ILogger<T> resolved from the container into a view model's constructor,
//         ILogger.BeginScope, and LogInformation with a structured message template.
// Passes: dotnet test --filter FullyQualifiedName~Ex039_

using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>Ready to use - one recorded log line, including which scopes were active when
/// it was written.</summary>
public sealed class Ex039_LogEntry
{
    public required LogLevel Level { get; init; }

    public required string Category { get; init; }

    public required string Message { get; init; }

    public required IReadOnlyList<string> Scopes { get; init; }
}

/// <summary>Ready to use - where recorded entries land, standing in for a real log sink.</summary>
public sealed class Ex039_RecordingSink
{
    public List<Ex039_LogEntry> Entries { get; } = [];
}

/// <summary>Ready to use - an ILoggerProvider that writes into an Ex039_RecordingSink
/// instead of the console or a file, so a test can assert on structured log calls and their
/// scopes.</summary>
public sealed class Ex039_RecordingLoggerProvider(Ex039_RecordingSink sink) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new Ex039_RecordingLogger(categoryName, sink);

    public void Dispose()
    {
    }

    private sealed class Ex039_RecordingLogger(string categoryName, Ex039_RecordingSink sink) : ILogger
    {
        private readonly Stack<string> _scopes = new();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            _scopes.Push(state.ToString() ?? string.Empty);
            return new PopScope(_scopes);
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            sink.Entries.Add(new Ex039_LogEntry
            {
                Level = logLevel,
                Category = categoryName,
                Message = formatter(state, exception),
                Scopes = _scopes.ToArray(),
            });
        }

        private sealed class PopScope(Stack<string> scopes) : IDisposable
        {
            public void Dispose() => scopes.Pop();
        }
    }
}

/// <summary>
/// A view model that logs the readings it records. Constructed with a real
/// ILogger&lt;Ex039_MeterReadingViewModel&gt; - how it gets that logger is not the subject
/// of this row (see Ex039_LoggingIntegration.BuildHost/ResolveViewModel below), but
/// RecordReading itself is.
/// </summary>
public sealed class Ex039_MeterReadingViewModel : INotifyPropertyChanged
{
    private readonly ILogger<Ex039_MeterReadingViewModel> _logger;
    private string _lastReadingSummary = string.Empty;

    public Ex039_MeterReadingViewModel(ILogger<Ex039_MeterReadingViewModel> logger)
    {
        _logger = logger;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Ready to use - what a real status TextBlock would bind to. RecordReading is
    /// responsible for keeping this current; it is not updated anywhere else.</summary>
    public string LastReadingSummary
    {
        get => _lastReadingSummary;
        private set
        {
            if (_lastReadingSummary == value) return;
            _lastReadingSummary = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastReadingSummary)));
        }
    }

    /// <summary>
    /// Records a meter reading. Every log line this produces must be tagged with a scope
    /// identifying meterId, so several readings from different meters interleaved in one
    /// log stream can still be told apart, and LastReadingSummary must reflect this
    /// reading afterward.
    /// </summary>
    public void RecordReading(string meterId, double value)
    {
        using (_logger.BeginScope("MeterId:{MeterId}", meterId))
        {
            _logger.LogInformation("Reading recorded: {Value}", value);
        }

        LastReadingSummary = $"{meterId}: {value}";
    }
}

public static class Ex039_LoggingIntegration
{
    /// <summary>Ready to use - builds a host with the recording provider wired in and
    /// Ex039_MeterReadingViewModel registered as transient.</summary>
    public static IHost BuildHost(Ex039_RecordingSink sink)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new Ex039_RecordingLoggerProvider(sink));
        builder.Services.AddTransient<Ex039_MeterReadingViewModel>();
        return builder.Build();
    }

    /// <summary>Ready to use.</summary>
    public static Ex039_MeterReadingViewModel ResolveViewModel(IHost host)
        => host.Services.GetRequiredService<Ex039_MeterReadingViewModel>();
}
