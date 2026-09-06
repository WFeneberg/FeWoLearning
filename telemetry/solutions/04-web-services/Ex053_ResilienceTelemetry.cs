using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 053 — ResilienceTelemetry (web-services).
// Goal:   Stop a retry policy from hiding the thing it is compensating for.
// Drills: a Polly retry pipeline, recording attempts as span events, counting retries
//         as a metric.
// Passes: a call that succeeds on the third attempt returns its value and leaves the
//                     span successful;
//         that span carries one "retry" event per RETRY, each naming its attempt number;
//         the retry counter records the same number, dimensioned by outcome;
//         a call that succeeds first time carries no retry events and moves no counter;
//         and a call that exhausts the policy leaves the span in Error and rethrows.
//
// The fourth clause is what makes the second one worth anything, and the pair is the
// row. A retry that eventually succeeds produces, at the caller, a result that is
// indistinguishable from a first-try success: same value, same status, same green span.
// So the dependency that failed twice out of every three calls looks perfectly healthy
// right up until it fails three times, at which point it appears to break instantly and
// without warning.
//
// The warning was always there. It was in the retries nobody recorded.
//
// Recording it twice - as span events and as a counter - is not redundancy. The events
// tell you what happened inside ONE call when you are looking at that call; the counter
// tells you the rate across all of them, which is the thing an alert can fire on. Neither
// answers the other's question.
//
// The last clause is row 018's lesson again, at the point where it is easiest to get
// wrong: a resilience pipeline that swallows the final failure has turned "we tried hard
// and could not" into "everything is fine".
public static class Ex053_ResilienceTelemetry
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex053";

    /// <summary>The meter this exercise emits from.</summary>
    public const string MeterName = "fewolearning.telemetry.ex053";

    /// <summary>The span wrapping the whole resilient call.</summary>
    public const string CallSpanName = "call.dependency";

    /// <summary>The event recorded once per retry.</summary>
    public const string RetryEventName = "retry";

    /// <summary>The tag on each retry event carrying which attempt it was.</summary>
    public const string AttemptTag = "retry.attempt";

    /// <summary>The counter of retries.</summary>
    public const string RetryCounter = "resilience.retries";

    /// <summary>The dimension carrying how the whole call ended.</summary>
    public const string OutcomeTag = "call.outcome";

    /// <summary>How many attempts the policy makes beyond the first.</summary>
    public const int MaxRetries = 3;

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>The one meter this exercise emits from.</summary>
    public static Meter Meter { get; } = new(MeterName);

    /// <summary>
    /// Build a provider recording <see cref="SourceName"/> into
    /// <paramref name="exported"/>. The caller disposes it.
    /// </summary>
    private static readonly Counter<long> Retries = Meter.CreateCounter<long>(
        RetryCounter, "{retry}", "Retries spent, by how the call ended.");

    public static TracerProvider Build(ICollection<Activity> exported) =>
        Sdk.CreateTracerProviderBuilder()
            .AddSource(SourceName)
            .AddInMemoryExporter(exported)
            .Build();

    /// <summary>
    /// Call <paramref name="work"/> inside a <see cref="CallSpanName"/> span, retrying a
    /// failure up to <see cref="MaxRetries"/> times.
    ///
    /// Record each RETRY - not each attempt - as an event named
    /// <see cref="RetryEventName"/> on the span, tagged <see cref="AttemptTag"/> with the
    /// number of the attempt about to be made, counting the first call as attempt 1.
    ///
    /// Then add the TOTAL number of retries to <see cref="RetryCounter"/> once, when the
    /// call is over, tagged <see cref="OutcomeTag"/> with "succeeded" or "failed".
    ///
    /// That ordering is forced rather than chosen, and it is worth noticing: the outcome
    /// is a dimension you do not know while the retries are happening, so the measurement
    /// has to wait until you do. A dimension learned late means a measurement recorded
    /// late - which is also why a counter with no retries records nothing at all rather
    /// than a zero.
    ///
    /// A call that ultimately succeeds returns its value. One that exhausts the policy
    /// sets the span's status to <see cref="ActivityStatusCode.Error"/> and lets the last
    /// exception out.
    /// </summary>
    public static async Task<T> CallAsync<T>(Func<int, Task<T>> work)
    {
        using var activity = Source.StartActivity(CallSpanName);

        var retries = 0;

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                var result = await work(attempt);

                // Recorded only when there were any: a counter that reports a zero on
                // every healthy call makes every call look degraded, and an alert on that
                // number fires constantly and gets muted.
                if (retries > 0) RecordRetries(retries, "succeeded");

                return result;
            }
            catch (Exception failure) when (attempt <= MaxRetries)
            {
                // A RETRY, not an attempt: the first call is not one. The tag names the
                // attempt about to be made, so the numbers read as "we are now trying
                // number 2".
                retries++;
                activity?.AddEvent(new ActivityEvent(
                    RetryEventName,
                    tags: new ActivityTagsCollection { { AttemptTag, attempt + 1 } }));

                _ = failure;
            }
            catch (Exception failure)
            {
                if (retries > 0) RecordRetries(retries, "failed");

                activity?.AddException(failure);
                activity?.SetStatus(ActivityStatusCode.Error, failure.Message);

                // A pipeline that swallows the final failure has turned "we tried hard and
                // could not" into "everything is fine".
                throw;
            }
        }
    }

    private static void RecordRetries(int count, string outcome) =>
        Retries.Add(count, new KeyValuePair<string, object?>(OutcomeTag, outcome));
}
