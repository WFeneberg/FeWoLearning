package fewolearning.exercises.advanced.ex077_rate_limiter;

import java.time.Clock;
import java.time.Duration;

/*
Exercise 077 - Rate limiter (advanced).

Goal:   Allow at most a fixed number of calls per time window, thread-safely.
Drills: time windows, thread safety.
*/
public final class RateLimiter {
    private final int maxCallsPerWindow;
    private final Duration window;
    private final Clock clock;

    public RateLimiter(int maxCallsPerWindow, Duration window, Clock clock) {
        this.maxCallsPerWindow = maxCallsPerWindow;
        this.window = window;
        this.clock = clock;
    }

    public synchronized boolean tryAcquire() {
        throw new UnsupportedOperationException("TODO");
    }
}
