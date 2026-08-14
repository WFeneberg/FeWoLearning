package fewolearning.exercises.intermediate.ex047_collector_partitioning;

import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

/*
Exercise 047 - Collector partitioning (reference solution).
*/
public final class CollectorPartitioning {
    private CollectorPartitioning() {
    }

    public static Map<Boolean, List<Integer>> partitionByEven(List<Integer> numbers) {
        return numbers.stream()
                .collect(Collectors.partitioningBy(number -> number % 2 == 0));
    }

    public static double averageOfEvens(List<Integer> numbers) {
        return numbers.stream()
                .filter(number -> number % 2 == 0)
                .mapToInt(Integer::intValue)
                .average()
                .orElse(0.0);
    }
}
