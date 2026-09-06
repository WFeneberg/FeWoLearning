using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

/// <summary>One record captured by <see cref="Ex008_CustomILoggerProvider"/>.</summary>
/// <param name="Category">The category of the logger that wrote it.</param>
/// <param name="Level">The level it was written at.</param>
/// <param name="Message">The rendered message.</param>
/// <param name="Scopes">The scopes active at the time, outermost first.</param>
public sealed record CapturedRecord(
    string Category, LogLevel Level, string Message, IReadOnlyList<object?> Scopes);

// Exercise 008 — CustomILoggerProvider (logging).
// Goal:   Write the sink side of the logging abstraction by hand, and learn what the
//         factory expects of a provider.
// Drills: ILoggerProvider, ILogger, ISupportExternalScope, IExternalScopeProvider.
// Passes: a record written through a logger from this provider is captured with the
//                     right category, level and rendered message;
//         two loggers with different categories each capture under their own;
//         the provider implements ISupportExternalScope;
//         and a scope opened on the factory's logger arrives in the captured record.
//
// The last two clauses are one mechanism seen from both sides, and getting only one of
// them is the classic half-finished provider.
//
// The factory INSPECTS each provider for ISupportExternalScope. If it finds it, it
// hands over one shared IExternalScopeProvider and from then on pushes scopes there,
// NOT into your logger's BeginScope - so a provider that advertises the interface and
// then ignores the object it is given sees no scopes at all, silently. If it does not
// find the interface, the factory falls back to calling your logger's BeginScope, and
// your provider ends up with a private scope stack that no other provider shares.
//
// Only one of those is right, and neither fails loudly.
public sealed class Ex008_CustomILoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly List<CapturedRecord> _captured = [];
    private IExternalScopeProvider? _scopes;

    /// <summary>Everything captured so far, oldest first.</summary>
    public IReadOnlyList<CapturedRecord> Captured
    {
        get { lock (_captured) return _captured.ToArray(); }
    }

    /// <summary>Create a logger that captures into <see cref="Captured"/>.</summary>
    public ILogger CreateLogger(string categoryName) => new CapturingLogger(this, categoryName);

    /// <summary>
    /// Called by the factory, once, before any logger is used. Keep
    /// <paramref name="scopeProvider"/> and read the active scopes out of it when a
    /// record is captured - that is the only place they will be.
    /// </summary>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider) => _scopes = scopeProvider;

    private void Capture(string category, LogLevel level, string message)
    {
        // Read the scope stack HERE, per record. Reading it once in SetScopeProvider
        // and caching the result would pin whatever happened to be open at startup
        // onto every record for the rest of the process.
        var scopes = new List<object?>();
        _scopes?.ForEachScope(static (scope, into) => into.Add(scope), scopes);

        lock (_captured) _captured.Add(new CapturedRecord(category, level, message, scopes));
    }

    private sealed class CapturingLogger(Ex008_CustomILoggerProvider owner, string category) : ILogger
    {
        // Returning null is correct here, and it is not laziness: because the provider
        // implements ISupportExternalScope, the factory pushes scopes into the shared
        // provider and never calls this. A private stack kept here would be a second,
        // divergent copy that no other provider in the pipeline can see.
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            // The formatter is the only thing that knows how to render this state.
            // Calling state.ToString() instead works by accident for some states and
            // silently produces a type name for others.
            owner.Capture(category, logLevel, formatter(state, exception));
        }
    }

    public void Dispose()
    {
        // Nothing to release. A real provider would flush and close its sink here, and
        // the factory does call this - it owns every provider added to it.
    }
}
