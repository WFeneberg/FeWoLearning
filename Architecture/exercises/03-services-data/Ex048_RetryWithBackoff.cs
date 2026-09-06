namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex048;

// Exercise 048 — RetryWithBackoff (services-data).
// Goal:   Retry a failing call in a way that gives the thing you are calling room to
//         recover, and that does not synchronise every caller onto the same instant.
// Drills: exponential backoff, jitter, retry budgets, testing delays without sleeping.
// Passes: DelayBefore(1) - zero. The first attempt is not a retry.
//         growth         - with no jitter, the delays are base, 2*base, 4*base, ...
//         jitter         - with a jitter fraction f, the delay stays inside
//                          [(1-f)*nominal, (1+f)*nominal] for ANY value the random source
//                          returns. Bounds, not equality - a jittered delay has no single
//                          correct value.
//         Execute        - retries until the work succeeds, sleeping the delays in order.
//         budget         - after maxAttempts failures the last exception propagates, and
//                          exactly maxAttempts - 1 sleeps happened.
//
// Two things are being drilled at once. Exponential growth, because a fixed delay just
// re-applies the same load to something that is already struggling. And jitter, because
// without it every caller that failed at the same moment - which is all of them, that is
// what an outage is - retries at the same moment, and the recovering service is hit by a
// synchronised wave exactly as it comes back.
//
// No test here sleeps: Execute takes the sleeper as a parameter, and the tests record
// what it was asked to wait for.
public sealed class RetryPolicy(TimeSpan baseDelay, int maxAttempts, double jitterFraction, Func<double> random)
{
    /// <summary>
    /// How long to wait before attempt <paramref name="attemptNumber"/> (1-based).
    /// Attempt 1 waits nothing; attempt n waits baseDelay * 2^(n-2), scaled by jitter.
    /// <paramref name="random"/> returns a value in [0, 1].
    /// </summary>
    public TimeSpan DelayBefore(int attemptNumber) =>
        throw new NotImplementedException(
            "TODO: Ex048 - zero before the first attempt, otherwise baseDelay doubled per retry and jittered by +/- jitterFraction");

    /// <summary>
    /// Run <paramref name="work"/>, retrying on any exception up to maxAttempts, asking
    /// <paramref name="sleep"/> to wait between attempts. The last failure propagates.
    /// </summary>
    public T Execute<T>(Func<T> work, Action<TimeSpan> sleep) =>
        throw new NotImplementedException(
            "TODO: Ex048 - attempt, and on failure sleep DelayBefore(next attempt) and try again until the budget runs out");
}
