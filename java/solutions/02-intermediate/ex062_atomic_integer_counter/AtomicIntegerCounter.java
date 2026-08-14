package fewolearning.exercises.intermediate.ex062_atomic_integer_counter;

import java.util.concurrent.atomic.AtomicInteger;

/*
Exercise 062 - Atomic integer counter (reference solution).
*/
public final class AtomicIntegerCounter {
    private final AtomicInteger count = new AtomicInteger();

    public int increment() {
        return count.incrementAndGet();
    }

    public int current() {
        return count.get();
    }

    public void reset() {
        count.set(0);
    }
}
