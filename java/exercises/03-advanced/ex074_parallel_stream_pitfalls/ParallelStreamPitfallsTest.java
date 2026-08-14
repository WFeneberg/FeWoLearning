package fewolearning.exercises.advanced.ex074_parallel_stream_pitfalls;

import java.util.ArrayList;
import java.util.List;
import java.util.stream.IntStream;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class ParallelStreamPitfallsTest {

    @Test
    void safeParallelSumAddsAllElementsWithoutASharedMutableAccumulator() {
        List<Integer> numbers = List.of(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

        assertEquals(55L, ParallelStreamPitfalls.safeParallelSum(numbers));
    }

    @Test
    void safeParallelSumHandlesALargeInputWithoutOverflowingAnInt() {
        List<Integer> numbers = new ArrayList<>();
        for (int i = 0; i < 100_000; i++) {
            numbers.add(Integer.MAX_VALUE / 1000);
        }
        long expected = numbers.stream().mapToLong(Integer::longValue).sum();

        assertEquals(expected, ParallelStreamPitfalls.safeParallelSum(numbers));
    }

    @Test
    void collectPreservingOrderReturnsElementsInTheirOriginalEncounterOrder() {
        List<Integer> numbers = IntStream.rangeClosed(1, 500).boxed().toList();

        List<Integer> result = ParallelStreamPitfalls.collectPreservingOrder(numbers);

        assertEquals(numbers, result);
    }
}
