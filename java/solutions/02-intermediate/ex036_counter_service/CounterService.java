package fewolearning.exercises.intermediate.ex036_counter_service;

/*
Exercise 036 - Counter service (reference solution).
*/
public final class CounterService {
    private int count;

    public int increment() {
        return ++count;
    }

    public int decrement() {
        if (count == 0) {
            throw new IllegalStateException("count cannot go below zero");
        }
        return --count;
    }

    public int current() {
        return count;
    }

    public void reset() {
        count = 0;
    }
}
