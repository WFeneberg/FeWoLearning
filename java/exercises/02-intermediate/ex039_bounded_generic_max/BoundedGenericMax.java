package fewolearning.exercises.intermediate.ex039_bounded_generic_max;

import java.util.List;

/*
Exercise 039 - Bounded generic max (intermediate).

Goal:   Find the largest element of any comparable list using a bounded type parameter.
Drills: bounds, Comparable, reusable algorithms.
*/
public final class BoundedGenericMax {
    private BoundedGenericMax() {
    }

    public static <T extends Comparable<T>> T max(List<T> values) {
        throw new UnsupportedOperationException("TODO");
    }
}
