package fewolearning.exercises.intermediate.ex047_collector_partitioning;

import java.util.List;
import java.util.Map;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class CollectorPartitioningTest {

    @Test
    void partitionByEvenSeparatesEvenAndOddNumbers() {
        Map<Boolean, List<Integer>> partitioned =
                CollectorPartitioning.partitionByEven(List.of(1, 2, 3, 4, 5, 6));

        assertEquals(List.of(2, 4, 6), partitioned.get(true));
        assertEquals(List.of(1, 3, 5), partitioned.get(false));
    }

    @Test
    void averageOfEvensComputesTheMeanOfEvenNumbers() {
        double average = CollectorPartitioning.averageOfEvens(List.of(1, 2, 3, 4, 5, 6));

        assertEquals(4.0, average, 1e-9);
    }

    @Test
    void averageOfEvensIsZeroWhenThereAreNoEvenNumbers() {
        double average = CollectorPartitioning.averageOfEvens(List.of(1, 3, 5));

        assertEquals(0.0, average, 1e-9);
    }
}
