package fewolearning.exercises.intermediate.ex060_synchronized_counter;

/*
Exercise 060 - Synchronized counter (reference solution).
*/
public final class SynchronizedCounter {
    private int count;

    public synchronized void increment() {
        count++;
    }

    public synchronized int current() {
        return count;
    }
}
