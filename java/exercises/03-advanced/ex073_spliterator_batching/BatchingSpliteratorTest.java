package fewolearning.exercises.advanced.ex073_spliterator_batching;

import java.util.ArrayList;
import java.util.List;
import java.util.Spliterator;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

class BatchingSpliteratorTest {

    @Test
    void groupsElementsIntoFixedSizeBatches() {
        List<Integer> source = List.of(1, 2, 3, 4, 5, 6);
        BatchingSpliterator<Integer> spliterator = new BatchingSpliterator<>(source.spliterator(), 2);

        List<List<Integer>> batches = drain(spliterator);

        assertEquals(List.of(List.of(1, 2), List.of(3, 4), List.of(5, 6)), batches);
    }

    @Test
    void theLastBatchMayBeSmallerThanTheBatchSize() {
        List<Integer> source = List.of(1, 2, 3, 4, 5);
        BatchingSpliterator<Integer> spliterator = new BatchingSpliterator<>(source.spliterator(), 2);

        List<List<Integer>> batches = drain(spliterator);

        assertEquals(List.of(List.of(1, 2), List.of(3, 4), List.of(5)), batches);
    }

    @Test
    void tryAdvanceReturnsFalseOnceTheSourceIsExhausted() {
        List<Integer> source = List.of(1, 2);
        BatchingSpliterator<Integer> spliterator = new BatchingSpliterator<>(source.spliterator(), 5);

        assertTrue(spliterator.tryAdvance(batch -> assertEquals(List.of(1, 2), batch)));
        assertFalse(spliterator.tryAdvance(batch -> { }));
    }

    @Test
    void trySplitDeclinesToSplit() {
        BatchingSpliterator<Integer> spliterator = new BatchingSpliterator<>(List.of(1, 2, 3).spliterator(), 2);

        assertNull(spliterator.trySplit());
    }

    @Test
    void characteristicsDoesNotClaimSized() {
        BatchingSpliterator<Integer> spliterator = new BatchingSpliterator<>(List.of(1, 2, 3).spliterator(), 2);

        assertEquals(0, spliterator.characteristics() & Spliterator.SIZED);
    }

    private static <T> List<List<T>> drain(Spliterator<List<T>> spliterator) {
        List<List<T>> result = new ArrayList<>();
        while (spliterator.tryAdvance(result::add)) {
            // collect every batch produced by the spliterator
        }
        return result;
    }
}
