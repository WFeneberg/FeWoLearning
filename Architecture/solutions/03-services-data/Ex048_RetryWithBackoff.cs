namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex048;

// Exercise 048 — RetryWithBackoff (reference solution).
public sealed class RetryPolicy(TimeSpan baseDelay, int maxAttempts, double jitterFraction, Func<double> random)
{
    public TimeSpan DelayBefore(int attemptNumber)
    {
        if (attemptNumber <= 1)
            return TimeSpan.Zero; // the first attempt is not a retry

        // 2^(n-2): attempt 2 waits base, attempt 3 waits 2*base, attempt 4 waits 4*base.
        // A fixed delay just re-applies the same load to something already struggling.
        var nominal = baseDelay.TotalMilliseconds * Math.Pow(2, attemptNumber - 2);

        // random() in [0,1] maps to a factor in [1-f, 1+f]. Without jitter every caller
        // that failed at the same moment - which is all of them, that is what an outage
        // is - retries at the same moment, and the recovering service is hit by a
        // synchronised wave exactly as it comes back.
        var factor = 1 - jitterFraction + (2 * jitterFraction * random());

        return TimeSpan.FromMilliseconds(nominal * factor);
    }

    public T Execute<T>(Func<T> work, Action<TimeSpan> sleep)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return work();
            }
            catch when (attempt < maxAttempts)
            {
                // The budget is what stops a retry loop becoming its own outage. Without
                // it, a permanently broken dependency turns every caller into an
                // infinite loop against it.
                sleep(DelayBefore(attempt + 1));
            }
        }
    }
}
