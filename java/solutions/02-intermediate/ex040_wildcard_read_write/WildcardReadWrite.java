package fewolearning.exercises.intermediate.ex040_wildcard_read_write;

import java.util.List;

/*
Exercise 040 - Wildcard read/write (reference solution).
*/
public final class WildcardReadWrite {
    private WildcardReadWrite() {
    }

    public static void copy(List<? extends Number> source, List<? super Number> destination) {
        for (Number value : source) {
            destination.add(value);
        }
    }

    public static double sum(List<? extends Number> numbers) {
        double total = 0.0;
        for (Number value : numbers) {
            total += value.doubleValue();
        }
        return total;
    }
}
