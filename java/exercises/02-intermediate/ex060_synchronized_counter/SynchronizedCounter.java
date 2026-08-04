package fewolearning.exercises.intermediate.ex060_synchronized_counter;

/*
Exercise 060 - Synchronized counter (intermediate).

Goal:   Increment a shared counter safely from multiple threads.
Drills: synchronized methods, race conditions.
*/
public final class SynchronizedCounter {
    private int count;

    public synchronized void increment() {
        throw new UnsupportedOperationException("TODO");
    }

    public synchronized int current() {
        throw new UnsupportedOperationException("TODO");
    }
}
