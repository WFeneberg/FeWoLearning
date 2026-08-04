package fewolearning.exercises.intermediate.ex037_generic_box;

/*
Exercise 037 - Generic box (intermediate).

Goal:   Store and retrieve a single value of any type, with an empty-box check.
Drills: generic classes, type parameters.
*/
public final class Box<T> {
    private T value;
    private boolean present;

    public void set(T value) {
        throw new UnsupportedOperationException("TODO");
    }

    public T get() {
        throw new UnsupportedOperationException("TODO");
    }

    public boolean isEmpty() {
        throw new UnsupportedOperationException("TODO");
    }
}
