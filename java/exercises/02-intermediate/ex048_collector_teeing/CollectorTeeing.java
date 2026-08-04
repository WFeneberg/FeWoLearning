package fewolearning.exercises.intermediate.ex048_collector_teeing;

import java.util.List;

/*
Exercise 048 - Collector teeing (intermediate).

Goal:   Compute the min and max of a list in a single pass using Collectors.teeing.
Drills: combining collectors, dual aggregation.
*/
public final class CollectorTeeing {
    private CollectorTeeing() {
    }

    public record MinMax(int min, int max) {
    }

    public static MinMax minAndMax(List<Integer> numbers) {
        throw new UnsupportedOperationException("TODO");
    }
}
