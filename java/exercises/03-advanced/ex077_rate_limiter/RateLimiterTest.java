package fewolearning.exercises.advanced.ex077_rate_limiter;

import java.time.Duration;
import java.time.Instant;
import java.time.ZoneId;
import java.time.ZoneOffset;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

class RateLimiterTest {

    @Test
    void allowsUpToTheMaximumCallsWithinAWindow() {
        MutableClock clock = new MutableClock(Instant.EPOCH, ZoneOffset.UTC);
        RateLimiter limiter = new RateLimiter(2, Duration.ofSeconds(1), clock);

        assertTrue(limiter.tryAcquire());
        assertTrue(limiter.tryAcquire());
        assertFalse(limiter.tryAcquire());
    }

    @Test
    void allowsMoreCallsOnceTheWindowHasElapsed() {
        MutableClock clock = new MutableClock(Instant.EPOCH, ZoneOffset.UTC);
        RateLimiter limiter = new RateLimiter(1, Duration.ofSeconds(1), clock);

        assertTrue(limiter.tryAcquire());
        assertFalse(limiter.tryAcquire());

        clock.advance(Duration.ofSeconds(1));

        assertTrue(limiter.tryAcquire());
    }

    @Test
    void doesNotResetTheWindowBeforeItHasFullyElapsed() {
        MutableClock clock = new MutableClock(Instant.EPOCH, ZoneOffset.UTC);
        RateLimiter limiter = new RateLimiter(1, Duration.ofSeconds(1), clock);

        assertTrue(limiter.tryAcquire());

        clock.advance(Duration.ofMillis(500));

        assertFalse(limiter.tryAcquire());
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
