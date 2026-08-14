package fewolearning.exercises.intermediate.ex042_comparator_then_compare;

import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

/*
Exercise 042 - Comparator thenComparing (reference solution).
*/
public final class ComparatorThenCompare {
    private ComparatorThenCompare() {
    }

    public record Employee(String department, String name, double salary) {
    }

    public static List<Employee> sort(List<Employee> employees) {
        Comparator<Employee> byDepartment = Comparator.comparing(Employee::department);
        Comparator<Employee> bySalaryDescending = Comparator.comparingDouble(Employee::salary).reversed();
        Comparator<Employee> byName = Comparator.comparing(Employee::name);

        return employees.stream()
                .sorted(byDepartment.thenComparing(bySalaryDescending).thenComparing(byName))
                .collect(Collectors.toList());
    }
}
