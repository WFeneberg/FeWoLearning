package fewolearning.exercises.intermediate.ex042_comparator_then_compare;

import java.util.List;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class ComparatorThenCompareTest {

    @Test
    void sortsByDepartmentThenSalaryDescendingThenName() {
        ComparatorThenCompare.Employee alice = new ComparatorThenCompare.Employee("Sales", "Alice", 50000);
        ComparatorThenCompare.Employee bob = new ComparatorThenCompare.Employee("Sales", "Bob", 60000);
        ComparatorThenCompare.Employee carl = new ComparatorThenCompare.Employee("Engineering", "Carl", 70000);
        ComparatorThenCompare.Employee dana = new ComparatorThenCompare.Employee("Sales", "Dana", 60000);

        List<ComparatorThenCompare.Employee> sorted =
                ComparatorThenCompare.sort(List.of(alice, bob, carl, dana));

        assertEquals(List.of(carl, bob, dana, alice), sorted);
    }

    @Test
    void doesNotMutateTheInputList() {
        ComparatorThenCompare.Employee alice = new ComparatorThenCompare.Employee("Sales", "Alice", 50000);
        ComparatorThenCompare.Employee bob = new ComparatorThenCompare.Employee("Engineering", "Bob", 60000);
        List<ComparatorThenCompare.Employee> input = List.of(alice, bob);

        ComparatorThenCompare.sort(input);

        assertEquals(List.of(alice, bob), input);
    }
}
