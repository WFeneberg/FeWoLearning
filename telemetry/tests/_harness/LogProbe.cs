using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// A <see cref="FakeLogger"/> factory whose records expose STRUCTURED state, not just
/// the rendered message. That distinction is the whole point of block 01: an
/// interpolated string and a message template produce byte-identical text and
/// completely different state.
/// </summary>
public sealed class LogProbe : IDisposable
{
    private readonly FakeLogCollector _collector = new();
    private readonly ILoggerFactory _factory;

    public LogProbe()
    {
        _factory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddProvider(new FakeLoggerProvider(_collector));
        });
    }

    public ILogger<T> For<T>() => _factory.CreateLogger<T>();

    public ILogger For(string category) => _factory.CreateLogger(category);

    /// <summary>Everything logged so far, oldest first.</summary>
    public IReadOnlyList<FakeLogRecord> Records => _collector.GetSnapshot();

    /// <summary>
    /// The value of one named field, or <c>null</c> when the record carries no such
    /// field. A record produced by an interpolated string carries NO named fields at
    /// all - so a null return is the signal that grades interpolation as wrong.
    /// </summary>
    public static string? Field(FakeLogRecord record, string name) =>
        record.StructuredState?.FirstOrDefault(kv => kv.Key == name).Value;

    /// <summary>
    /// The constant template behind a record. Identical across calls with different
    /// argument values when - and only when - a message template was used.
    /// </summary>
    public static string? OriginalFormat(FakeLogRecord record) =>
        Field(record, "{OriginalFormat}");

    /// <summary>
    /// The raw scope objects active when the record was written, outermost first.
    ///
    /// Measured 2026-09-06 on Microsoft.Extensions.Diagnostics.Testing 10.9.0:
    /// FakeLogger captures these with NO IncludeScopes opt-in, and does NOT flatten
    /// them - a dictionary scope arrives as the dictionary itself, not as named
    /// fields on the record. A fact written as though scopes arrive pre-flattened
    /// fails against a correct implementation.
    /// </summary>
    public static IReadOnlyList<object?> Scopes(FakeLogRecord record) => record.Scopes;

    public void Dispose() => _factory.Dispose();
}
