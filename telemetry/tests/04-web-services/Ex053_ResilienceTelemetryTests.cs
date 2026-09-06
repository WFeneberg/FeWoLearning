using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex053_ResilienceTelemetryTests
{
    /// <summary>Fails the first <c>failures</c> attempts, then succeeds.</summary>
    private static Func<int, Task<string>> FailsThenSucceeds(int failures) =>
        attempt => attempt <= failures
            ? Task.FromException<string>(new InvalidOperationException($"attempt {attempt} failed"))
            : Task.FromResult($"ok on {attempt}");

    private static async Task<(Activity Span, IReadOnlyList<MetricPointSnapshot> Metrics, string? Result, Exception? Failure)>
        Call(Func<int, Task<string>> work)
    {
        var exported = new List<Activity>();

        using var metrics = new MetricProbe(Ex053_ResilienceTelemetry.MeterName);
        using var provider = Ex053_ResilienceTelemetry.Build(exported);

        string? result = null;
        Exception? failure = null;

        try
        {
            result = await Ex053_ResilienceTelemetry.CallAsync(work);
        }
        catch (Exception thrown)
        {
            failure = thrown;
        }

        provider.ForceFlush();

        return (Assert.Single(exported), metrics.Collect(), result, failure);
    }

    [Fact]
    public async Task A_call_that_succeeds_on_the_third_attempt_returns_its_value()
    {
        using var ctx = new TelemetryContext();

        var (span, _, result, failure) = await Call(FailsThenSucceeds(2));

        Assert.Null(failure);
        Assert.Equal("ok on 3", result);
        Assert.NotEqual(ActivityStatusCode.Error, span.Status);
    }

    [Fact]
    public async Task Adversarial_A_The_retries_are_on_the_span_even_though_the_call_succeeded()
    {
        // The row. A retry that eventually succeeds produces, at the caller, a result
        // indistinguishable from a first-try success: same value, same status, same green
        // span. So the dependency failing two calls in three looks perfectly healthy right
        // up until it fails three times, at which point it appears to break instantly and
        // without warning.
        //
        // The warning was always there. It was in the retries nobody recorded.
        using var ctx = new TelemetryContext();

        var (span, _, _, _) = await Call(FailsThenSucceeds(2));

        var retries = span.Events
            .Where(e => e.Name == Ex053_ResilienceTelemetry.RetryEventName)
            .ToArray();

        Assert.Equal(2, retries.Length);
        Assert.Equal(
            ["2", "3"],
            retries.Select(e =>
                e.Tags.Single(t => t.Key == Ex053_ResilienceTelemetry.AttemptTag).Value?.ToString()));
    }

    [Fact]
    public async Task Adversarial_B_A_first_try_success_records_nothing_at_all()
    {
        // The paired half, and what makes Adversarial_A worth anything. An implementation
        // that records a "retry" for every attempt makes every healthy call look
        // degraded - and an alert on that number fires constantly and gets muted.
        using var ctx = new TelemetryContext();

        var (span, metrics, result, _) = await Call(FailsThenSucceeds(0));

        Assert.Equal("ok on 1", result);
        Assert.DoesNotContain(span.Events, e => e.Name == Ex053_ResilienceTelemetry.RetryEventName);
        Assert.DoesNotContain(metrics, p => p.Instrument == Ex053_ResilienceTelemetry.RetryCounter);
    }

    [Fact]
    public async Task The_retry_counter_records_the_total_against_the_outcome()
    {
        // The events tell you what happened inside ONE call when you are looking at that
        // call; the counter tells you the rate across all of them, which is what an alert
        // fires on. Neither answers the other's question.
        using var ctx = new TelemetryContext();

        var (_, metrics, _, _) = await Call(FailsThenSucceeds(2));

        var point = Assert.Single(metrics, p => p.Instrument == Ex053_ResilienceTelemetry.RetryCounter);
        Assert.Equal(2d, point.Sum);
        Assert.Equal("succeeded", point.Tag(Ex053_ResilienceTelemetry.OutcomeTag));
    }

    [Fact]
    public async Task Adversarial_C_An_exhausted_policy_fails_loudly()
    {
        // Row 018's lesson at the point where it is easiest to get wrong: a resilience
        // pipeline that swallows the final failure has turned "we tried hard and could
        // not" into "everything is fine".
        using var ctx = new TelemetryContext();

        var (span, metrics, result, failure) = await Call(FailsThenSucceeds(99));

        Assert.Null(result);
        Assert.IsType<InvalidOperationException>(failure);
        Assert.Equal(ActivityStatusCode.Error, span.Status);

        var point = Assert.Single(metrics, p => p.Instrument == Ex053_ResilienceTelemetry.RetryCounter);
        Assert.Equal(Ex053_ResilienceTelemetry.MaxRetries, point.Sum);
        Assert.Equal("failed", point.Tag(Ex053_ResilienceTelemetry.OutcomeTag));
    }
}
