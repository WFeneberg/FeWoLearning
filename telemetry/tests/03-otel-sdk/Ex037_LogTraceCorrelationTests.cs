using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry.Logs;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex037_LogTraceCorrelationTests
{
    [Fact]
    public void A_record_written_inside_a_span_carries_that_spans_ids()
    {
        // Correlation is not a parameter and not a convention everyone has to remember:
        // it is read from Activity.Current when the record is created. That is what makes
        // it survive code you did not write - a library logging inside your request gets
        // your trace id without knowing your application exists.
        using var ctx = new TelemetryContext();
        var exported = new List<LogRecord>();

        string? traceId;
        var (logs, traces) = Ex037_LogTraceCorrelation.Build(exported);
        using (logs)
        using (traces)
        {
            traceId = Ex037_LogTraceCorrelation.LogInsideASpan(
                logs.CreateLogger(Ex037_LogTraceCorrelation.CategoryName), "acme");
        }

        Assert.NotNull(traceId);
        var record = Assert.Single(exported);
        Assert.Equal(traceId, record.TraceId.ToHexString());
        Assert.NotEqual(default, record.SpanId);
        Assert.Equal(ActivityTraceFlags.Recorded, record.TraceFlags);
    }

    [Fact]
    public void Adversarial_A_A_record_written_outside_every_span_carries_zeros()
    {
        // What stops correlation being a lie. Outside a span there is no trace, so the
        // SDK writes zeros rather than inventing an id that points nowhere - and a
        // backend reads all-zero as "unlinked" rather than as a trace it has lost.
        using var ctx = new TelemetryContext();
        var exported = new List<LogRecord>();

        var (logs, traces) = Ex037_LogTraceCorrelation.Build(exported);
        using (logs)
        using (traces)
        {
            Ex037_LogTraceCorrelation.LogOutsideAnySpan(
                logs.CreateLogger(Ex037_LogTraceCorrelation.CategoryName));
        }

        var record = Assert.Single(exported);
        Assert.Equal(default, record.TraceId);
        Assert.Equal(default, record.SpanId);
    }

    [Fact]
    public void Adversarial_B_The_same_call_site_reports_two_different_traces()
    {
        // The proof that the id comes from ambient context rather than from anything the
        // call site knows: nothing about the call changed between these two.
        using var ctx = new TelemetryContext();
        var exported = new List<LogRecord>();

        string? first, second;
        var (logs, traces) = Ex037_LogTraceCorrelation.Build(exported);
        using (logs)
        using (traces)
        {
            var logger = logs.CreateLogger(Ex037_LogTraceCorrelation.CategoryName);
            first = Ex037_LogTraceCorrelation.LogInsideASpan(logger, "acme");
            second = Ex037_LogTraceCorrelation.LogInsideASpan(logger, "acme");
        }

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.NotEqual(first, second);
        Assert.Equal([first, second], exported.Select(r => r.TraceId.ToHexString()));
    }

    [Fact]
    public void Adversarial_C_Scopes_reach_the_record_only_when_the_pipeline_asked_for_them()
    {
        // A difference worth carrying away, because two libraries in this repository
        // disagree: FakeLogger in block 01 captures scopes unconditionally, and this
        // pipeline drops them silently unless told otherwise. Neither says so.
        using var ctx = new TelemetryContext();

        Assert.Empty(ScopesFrom(includeScopes: false));
        Assert.Equal(
            [$"{Ex037_LogTraceCorrelation.TenantScopeKey}=acme"],
            ScopesFrom(includeScopes: true));
    }

    private static IReadOnlyList<string> ScopesFrom(bool includeScopes)
    {
        var exported = new List<LogRecord>();

        var (logs, traces) = Ex037_LogTraceCorrelation.Build(exported, includeScopes);
        using (logs)
        using (traces)
        {
            Ex037_LogTraceCorrelation.LogInsideASpan(
                logs.CreateLogger(Ex037_LogTraceCorrelation.CategoryName), "acme");
        }

        return LogRecordReadout.Scopes(Assert.Single(exported));
    }
}
