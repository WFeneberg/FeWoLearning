package fewolearning.exercises.intermediate.ex036_counter_service;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class CounterServiceTest {

    @Test
    void incrementsFromZero() {
        CounterService counter = new CounterService();

        assertEquals(1, counter.increment());
        assertEquals(2, counter.increment());
    }

    @Test
    void decrementsAnIncrementedCounter() {
        CounterService counter = new CounterService();
        counter.increment();
        counter.increment();

        assertEquals(1, counter.decrement());
    }

    @Test
    void currentReflectsTheLatestCount() {
        CounterService counter = new CounterService();
        counter.increment();
        counter.increment();
        counter.increment();

        assertEquals(3, counter.current());
    }

    @Test
    void resetReturnsTheCountToZero() {
        CounterService counter = new CounterService();
        counter.increment();

        counter.reset();

        assertEquals(0, counter.current());
    }

    @Test
    void decrementThrowsWhenCountIsAlreadyZero() {
        CounterService counter = new CounterService();

        assertThrows(IllegalStateException.class, counter::decrement);
    }
}
