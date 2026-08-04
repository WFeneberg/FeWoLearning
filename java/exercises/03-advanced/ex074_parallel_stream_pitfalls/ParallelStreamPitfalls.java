package fewolearning.exercises.advanced.ex074_parallel_stream_pitfalls;

import java.util.List;

/*
Exercise 074 - Parallel stream pitfalls (advanced).

Goal:   Sum a list in parallel without a shared mutable accumulator.
Drills: parallel streams, statefulness, ordering.
*/
public final class ParallelStreamPitfalls {
    private ParallelStreamPitfalls() {
    }

    public static long safeParallelSum(List<Integer> numbers) {
        throw new UnsupportedOperationException("TODO");
    }

    public static List<Integer> collectPreservingOrder(List<Integer> numbers) {
        throw new UnsupportedOperationException("TODO");
    }
}
