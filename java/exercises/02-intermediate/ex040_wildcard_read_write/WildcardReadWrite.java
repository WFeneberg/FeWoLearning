package fewolearning.exercises.intermediate.ex040_wildcard_read_write;

import java.util.List;

/*
Exercise 040 - Wildcard read/write (intermediate).

Goal:   Copy elements from a producer list into a consumer list using PECS wildcards.
Drills: PECS, wildcards, variance intuition.
*/
public final class WildcardReadWrite {
    private WildcardReadWrite() {
    }

    public static void copy(List<? extends Number> source, List<? super Number> destination) {
        throw new UnsupportedOperationException("TODO");
    }

    public static double sum(List<? extends Number> numbers) {
        throw new UnsupportedOperationException("TODO");
    }
}
