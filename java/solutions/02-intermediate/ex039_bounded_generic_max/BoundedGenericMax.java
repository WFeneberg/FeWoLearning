package fewolearning.exercises.intermediate.ex039_bounded_generic_max;

import java.util.List;
import java.util.NoSuchElementException;

/*
Exercise 039 - Bounded generic max (reference solution).
*/
public final class BoundedGenericMax {
    private BoundedGenericMax() {
    }

    public static <T extends Comparable<T>> T max(List<T> values) {
        if (values.isEmpty()) {
            throw new NoSuchElementException("cannot find max of an empty list");
        }
        T best = values.get(0);
        for (T value : values) {
            if (value.compareTo(best) > 0) {
                best = value;
            }
        }
        return best;
    }
}
