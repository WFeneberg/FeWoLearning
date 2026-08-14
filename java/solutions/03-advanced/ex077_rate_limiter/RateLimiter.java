package fewolearning.exercises.advanced.ex077_rate_limiter;

import java.time.Clock;
import java.time.Duration;
import java.time.Instant;

/*
Exercise 077 - Rate limiter (reference solution).
*/
public final class RateLimiter {
    private final int maxCallsPerWindow;
    private final Duration window;
    private final Clock clock;
    private Instant windowStart;
    private int callsInWindow;

    public RateLimiter(int maxCallsPerWindow, Duration window, Clock clock) {
        this.maxCallsPerWindow = maxCallsPerWindow;
        this.window = window;
        this.clock = clock;
        this.windowStart = clock.instant();
        this.callsInWindow = 0;
    }

    public synchronized boolean tryAcquire() {
        Instant now = clock.instant();
        if (Duration.between(windowStart, now).compareTo(window) >= 0) {
            windowStart = now;
            callsInWindow = 0;
        }
        if (callsInWindow < maxCallsPerWindow) {
            callsInWindow++;
            return true;
        }
        return false;
    }
}
