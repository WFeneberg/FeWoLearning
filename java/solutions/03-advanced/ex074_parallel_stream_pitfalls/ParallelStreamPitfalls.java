package fewolearning.exercises.advanced.ex074_parallel_stream_pitfalls;

import java.util.List;
import java.util.stream.Collectors;

/*
Exercise 074 - Parallel stream pitfalls (reference solution).
*/
public final class ParallelStreamPitfalls {
    private ParallelStreamPitfalls() {
    }

    public static long safeParallelSum(List<Integer> numbers) {
        return numbers.parallelStream().mapToLong(Integer::longValue).sum();
    }

    public static List<Integer> collectPreservingOrder(List<Integer> numbers) {
        return numbers.parallelStream().collect(Collectors.toList());
    }
}
