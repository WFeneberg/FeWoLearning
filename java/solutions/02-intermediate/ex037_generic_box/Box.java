package fewolearning.exercises.intermediate.ex037_generic_box;

/*
Exercise 037 - Generic box (reference solution).
*/
public final class Box<T> {
    private T value;
    private boolean present;

    public void set(T value) {
        this.value = value;
        this.present = true;
    }

    public T get() {
        if (!present) {
            throw new IllegalStateException("box is empty");
        }
        return value;
    }

    public boolean isEmpty() {
        return !present;
    }
}
