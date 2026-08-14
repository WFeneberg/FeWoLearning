package fewolearning.exercises.advanced.ex088_retry_backoff;

import java.time.Duration;
import java.util.concurrent.Callable;

/*
Exercise 088 - Retry with backoff (reference solution).
*/
public final class RetryBackoff {
    private RetryBackoff() {
    }

    public static <T> T withRetry(Callable<T> operation, int maxAttempts, Duration initialDelay) throws Exception {
        Duration delay = initialDelay;
        Exception lastException = null;
        for (int attempt = 1; attempt <= maxAttempts; attempt++) {
            try {
                return operation.call();
            } catch (Exception e) {
                lastException = e;
                if (attempt == maxAttempts) {
                    break;
                }
                Thread.sleep(delay.toMillis());
                delay = delay.multipliedBy(2);
            }
        }
        throw lastException;
    }
}
