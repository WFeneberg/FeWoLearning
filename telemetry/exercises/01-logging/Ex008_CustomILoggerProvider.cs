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
    /// <summary>Everything captured so far, oldest first.</summary>
    public IReadOnlyList<CapturedRecord> Captured =>
        throw new NotImplementedException("TODO: Ex008 - expose the captured records");

    /// <summary>Create a logger that captures into <see cref="Captured"/>.</summary>
    public ILogger CreateLogger(string categoryName) =>
        throw new NotImplementedException("TODO: Ex008 - return a logger that captures under this category");

    /// <summary>
    /// Called by the factory, once, before any logger is used. Keep
    /// <paramref name="scopeProvider"/> and read the active scopes out of it when a
    /// record is captured - that is the only place they will be.
    /// </summary>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider) =>
        throw new NotImplementedException("TODO: Ex008 - remember the scope provider the factory supplies");

    public void Dispose()
    {
        // Nothing to release. A real provider would flush and close its sink here, and
        // the factory does call this - it owns every provider added to it.
    }
}
