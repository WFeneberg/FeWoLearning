package fewolearning.exercises.advanced.ex088_retry_backoff;

import java.time.Duration;
import java.util.concurrent.Callable;

/*
Exercise 088 - Retry with backoff (advanced).

Goal:   Retry a failing operation with increasing delay between attempts.
Drills: retries, jitter/backoff policy.
*/
public final class RetryBackoff {
    private RetryBackoff() {
    }

    public static <T> T withRetry(Callable<T> operation, int maxAttempts, Duration initialDelay) throws Exception {
        throw new UnsupportedOperationException("TODO");
    }
}
