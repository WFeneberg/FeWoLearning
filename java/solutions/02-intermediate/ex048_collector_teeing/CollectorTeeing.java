package fewolearning.exercises.intermediate.ex048_collector_teeing;

import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

/*
Exercise 048 - Collector teeing (reference solution).
*/
public final class CollectorTeeing {
    private CollectorTeeing() {
    }

    public record MinMax(int min, int max) {
    }

    public static MinMax minAndMax(List<Integer> numbers) {
        return numbers.stream().collect(Collectors.teeing(
                Collectors.minBy(Comparator.naturalOrder()),
                Collectors.maxBy(Comparator.naturalOrder()),
                (min, max) -> new MinMax(min.orElseThrow(), max.orElseThrow())));
    }
}
