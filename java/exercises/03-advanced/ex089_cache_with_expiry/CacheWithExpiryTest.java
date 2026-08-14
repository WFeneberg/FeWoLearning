package fewolearning.exercises.advanced.ex089_cache_with_expiry;

import java.time.Duration;
import java.time.Instant;
import java.time.ZoneId;
import java.time.ZoneOffset;
import java.util.concurrent.atomic.AtomicInteger;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class CacheWithExpiryTest {

    @Test
    void loadsTheValueOnTheFirstCall() {
        MutableClock clock = new MutableClock(Instant.EPOCH, ZoneOffset.UTC);
        CacheWithExpiry<String> cache = new CacheWithExpiry<>(clock, Duration.ofSeconds(10));
        AtomicInteger loadCount = new AtomicInteger();

        String value = cache.get(() -> "value-" + loadCount.incrementAndGet());

        assertEquals("value-1", value);
        assertEquals(1, loadCount.get());
    }

    @Test
    void reusesTheCachedValueWhileItHasNotExpired() {
        MutableClock clock = new MutableClock(Instant.EPOCH, ZoneOffset.UTC);
        CacheWithExpiry<String> cache = new CacheWithExpiry<>(clock, Duration.ofSeconds(10));
        AtomicInteger loadCount = new AtomicInteger();

        cache.get(() -> "value-" + loadCount.incrementAndGet());
        clock.advance(Duration.ofSeconds(5));
        String second = cache.get(() -> "value-" + loadCount.incrementAndGet());

        assertEquals("value-1", second);
        assertEquals(1, loadCount.get());
    }

    @Test
    void recomputesTheValueOnceItHasExpired() {
        MutableClock clock = new MutableClock(Instant.EPOCH, ZoneOffset.UTC);
        CacheWithExpiry<String> cache = new CacheWithExpiry<>(clock, Duration.ofSeconds(10));
        AtomicInteger loadCount = new AtomicInteger();

        cache.get(() -> "value-" + loadCount.incrementAndGet());
        clock.advance(Duration.ofSeconds(10));
        String second = cache.get(() -> "value-" + loadCount.incrementAndGet());

        assertEquals("value-2", second);
        assertEquals(2, loadCount.get());
    }

    static final class MutableClock extends java.time.Clock {
        private Instant instant;
        private final ZoneId zone;

        MutableClock(Instant instant, ZoneId zone) {
            this.instant = instant;
            this.zone = zone;
        }

        @Override
        public ZoneId getZone() {
            return zone;
        }

        @Override
        public java.time.Clock withZone(ZoneId zone) {
            return new MutableClock(instant, zone);
        }

        @Override
        public Instant instant() {
            return instant;
        }

        void advance(Duration duration) {
            instant = instant.plus(duration);
        }
    }
}
