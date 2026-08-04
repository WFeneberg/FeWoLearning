package fewolearning.exercises.advanced.ex089_cache_with_expiry;

import java.time.Clock;
import java.time.Duration;
import java.util.function.Supplier;

/*
Exercise 089 - Cache with expiry (advanced).

Goal:   Cache a value for a fixed duration and recompute once it has expired.
Drills: clocks, stale data, synchronization.
*/
public final class CacheWithExpiry<T> {
    private final Clock clock;
    private final Duration timeToLive;

    public CacheWithExpiry(Clock clock, Duration timeToLive) {
        this.clock = clock;
        this.timeToLive = timeToLive;
    }

    public synchronized T get(Supplier<T> loader) {
        throw new UnsupportedOperationException("TODO");
    }
}
