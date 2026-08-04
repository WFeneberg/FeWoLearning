package fewolearning.exercises.intermediate.ex042_comparator_then_compare;

import java.util.List;

/*
Exercise 042 - Comparator thenComparing (intermediate).

Goal:   Sort employees by department, then by salary descending, then by name.
Drills: comparator chaining, derived sort keys.
*/
public final class ComparatorThenCompare {
    private ComparatorThenCompare() {
    }

    public record Employee(String department, String name, double salary) {
    }

    public static List<Employee> sort(List<Employee> employees) {
        throw new UnsupportedOperationException("TODO");
    }
}
