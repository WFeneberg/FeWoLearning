using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 037 — LogTraceCorrelation (otel-sdk).
// Goal:   Get from a log line to the trace it belongs to without anyone having to pass
//         an id around.
// Drills: LogRecord.TraceId, the ambient Activity, IncludeScopes.
// Passes: a record written inside a span carries that span's trace id, its span id and
//                     the recorded flag - with nothing in the logging call mentioning
//                     any of it;
//         a record written outside every span carries all-zero ids rather than an
//                     invented one;
//         two different spans produce two different trace ids from the SAME call site;
//         and scopes reach the record only when the pipeline was built with
//                     IncludeScopes.
//
// The first and third clauses together are the point. Correlation is not a parameter and
// not a convention that everyone has to remember - it is read from Activity.Current at
// the moment the record is created. That is what makes it survive code you did not
// write: a library logging inside your request gets your trace id without knowing your
// application exists.
//
// The second clause is what stops that being a lie. Outside a span there is no trace, so
// the SDK writes zeros rather than inventing an id that points nowhere - and a backend
// reads all-zero as "unlinked" rather than as a trace it has lost.
//
// The last clause is a difference worth carrying away, because two libraries in this same
// repository disagree: FakeLogger in block 01 captures scopes unconditionally, and this
// pipeline drops them silently unless asked. Neither tells you. Measured 2026-09-06.
public static class Ex037_LogTraceCorrelation
{
    /// <summary>The category the exercise logs under.</summary>
    public const string CategoryName = "fewolearning.telemetry.ex037";

    /// <summary>The source whose spans the records point at.</summary>
    public const string SourceName = "fewolearning.telemetry.ex037";

    /// <summary>The scope key carrying the tenant.</summary>
    public const string TenantScopeKey = "TenantId";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build the two providers this row needs: a tracer provider recording
    /// <see cref="SourceName"/>, and a logger factory exporting into
    /// <paramref name="exported"/>.
    ///
    /// <paramref name="includeScopes"/> decides whether scopes travel with the records.
    ///
    /// The caller disposes both.
    /// </summary>
    public static (ILoggerFactory Logs, TracerProvider Traces) Build(
        ICollection<LogRecord> exported, bool includeScopes = false)
    {
        var traces = Sdk.CreateTracerProviderBuilder()
            .AddSource(SourceName)
            .Build();

        var logs = LoggerFactory.Create(builder => builder
            .SetMinimumLevel(LogLevel.Trace)
            .AddOpenTelemetry(options =>
            {
                // Nothing here mentions tracing. The correlation is not configured -
                // it is read from Activity.Current when each record is created.
                options.IncludeScopes = includeScopes;
                options.AddInMemoryExporter(exported);
            }));

        return (logs, traces);
    }

    /// <summary>
    /// Open a span, write ONE Information record reading "working" inside it - wrapped
    /// in a scope carrying <see cref="TenantScopeKey"/> - and return the span's trace id
    /// as lowercase hex, or null if nothing was listening.
    ///
    /// The logging call must not mention the trace in any way.
    /// </summary>
    public static string? LogInsideASpan(ILogger logger, string tenantId)
    {
        using var activity = Source.StartActivity("work");

        using (logger.BeginScope(new Dictionary<string, object> { [TenantScopeKey] = tenantId }))
        {
            // No trace id, no span id, no correlation argument of any kind.
            logger.LogInformation("working");
        }

        return activity?.TraceId.ToHexString();
    }

    /// <summary>Write ONE Information record reading "working" with no span in scope.</summary>
    public static void LogOutsideAnySpan(ILogger logger) => logger.LogInformation("working");
}
