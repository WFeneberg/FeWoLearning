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

    /// <param name="configure">
    /// Applied AFTER the defaults, so an exercise's own filter rules win over the
    /// probe's permissive baseline. Leave it null and everything from Trace upwards
    /// is captured - which is what a fact about content, rather than about filtering,
    /// wants.
    /// </param>
    public LogProbe(Action<ILoggingBuilder>? configure = null)
    {
        _factory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddProvider(new FakeLoggerProvider(_collector));
            configure?.Invoke(builder);
        });
    }

    /// <summary>The factory itself, for exercises whose subject is category naming.</summary>
    public ILoggerFactory Factory => _factory;

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

    /// <summary>
    /// One named value out of the scope at <paramref name="index"/> (0 is the
    /// outermost), or <c>null</c> when that scope has no such key.
    ///
    /// Handles both idiomatic shapes: a <c>Dictionary&lt;string, object&gt;</c> scope
    /// and a <c>BeginScope("Tenant {TenantId}", id)</c> scope, which the logging
    /// abstraction turns into a key/value sequence carrying the same names. Both
    /// erase to IEnumerable&lt;KeyValuePair&lt;string, object&gt;&gt; at runtime.
    /// </summary>
    public static string? ScopeValue(FakeLogRecord record, int index, string key)
    {
        if (index < 0 || index >= record.Scopes.Count) return null;

        return record.Scopes[index] is IEnumerable<KeyValuePair<string, object?>> pairs
            ? pairs.FirstOrDefault(kv => kv.Key == key).Value?.ToString()
            : null;
    }

    public void Dispose() => _factory.Dispose();
}
