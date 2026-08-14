package fewolearning.exercises.advanced.ex088_retry_backoff;

import java.time.Duration;
import java.util.concurrent.Callable;
import java.util.concurrent.atomic.AtomicInteger;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class RetryBackoffTest {

    @Test
    void retriesUntilTheOperationSucceedsAndReturnsItsValue() throws Exception {
        AtomicInteger callCount = new AtomicInteger();
        Callable<String> operation = () -> {
            int attempt = callCount.incrementAndGet();
            if (attempt < 3) {
                throw new RuntimeException("attempt " + attempt + " failed");
            }
            return "success";
        };

        String result = RetryBackoff.withRetry(operation, 5, Duration.ofMillis(1));

        assertEquals("success", result);
        assertEquals(3, callCount.get());
    }

    @Test
    void succeedsOnTheFirstAttemptWithoutAnyRetries() throws Exception {
        AtomicInteger callCount = new AtomicInteger();
        Callable<String> operation = () -> {
            callCount.incrementAndGet();
            return "first try";
        };

        String result = RetryBackoff.withRetry(operation, 3, Duration.ofMillis(1));

        assertEquals("first try", result);
        assertEquals(1, callCount.get());
    }

    @Test
    void throwsTheLastExceptionAfterExhaustingAllAttempts() {
        AtomicInteger callCount = new AtomicInteger();
        Callable<String> operation = () -> {
            callCount.incrementAndGet();
            throw new RuntimeException("always fails");
        };

        Exception thrown = assertThrows(RuntimeException.class,
                () -> RetryBackoff.withRetry(operation, 4, Duration.ofMillis(1)));

        assertEquals("always fails", thrown.getMessage());
        assertEquals(4, callCount.get());
    }
}
