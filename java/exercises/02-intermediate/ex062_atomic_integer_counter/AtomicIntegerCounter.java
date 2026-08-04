package fewolearning.exercises.intermediate.ex062_atomic_integer_counter;

import java.util.concurrent.atomic.AtomicInteger;

/*
Exercise 062 - Atomic integer counter (intermediate).

Goal:   Increment and reset a counter without locks using AtomicInteger.
Drills: atomics, lock-free increments.
*/
public final class AtomicIntegerCounter {
    private final AtomicInteger count = new AtomicInteger();

    public int increment() {
        throw new UnsupportedOperationException("TODO");
    }

    public int current() {
        throw new UnsupportedOperationException("TODO");
    }

    public void reset() {
        throw new UnsupportedOperationException("TODO");
    }
}
