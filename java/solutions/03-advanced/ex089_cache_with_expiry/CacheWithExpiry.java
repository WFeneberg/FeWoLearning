package fewolearning.exercises.advanced.ex089_cache_with_expiry;

import java.time.Clock;
import java.time.Duration;
import java.time.Instant;
import java.util.function.Supplier;

/*
Exercise 089 - Cache with expiry (reference solution).
*/
public final class CacheWithExpiry<T> {
    private final Clock clock;
    private final Duration timeToLive;
    private T cachedValue;
    private Instant expiresAt;
    private boolean hasValue;

    public CacheWithExpiry(Clock clock, Duration timeToLive) {
        this.clock = clock;
        this.timeToLive = timeToLive;
    }

    public synchronized T get(Supplier<T> loader) {
        Instant now = clock.instant();
        if (!hasValue || !now.isBefore(expiresAt)) {
            cachedValue = loader.get();
            expiresAt = now.plus(timeToLive);
            hasValue = true;
        }
        return cachedValue;
    }
}
