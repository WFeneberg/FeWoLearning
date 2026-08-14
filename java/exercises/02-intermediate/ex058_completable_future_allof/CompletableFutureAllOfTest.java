package fewolearning.exercises.intermediate.ex058_completable_future_allof;

import java.util.List;
import java.util.concurrent.CompletableFuture;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class CompletableFutureAllOfTest {

    @Test
    void collectAllReturnsResultsInInputOrder() throws Exception {
        List<CompletableFuture<Integer>> futures = List.of(
                CompletableFuture.completedFuture(1),
                CompletableFuture.completedFuture(2),
                CompletableFuture.completedFuture(3));

        CompletableFuture<List<Integer>> combined = CompletableFutureAllOf.collectAll(futures);

        assertEquals(List.of(1, 2, 3), combined.get());
    }

    @Test
    void collectAllHandlesASingleFuture() throws Exception {
        List<CompletableFuture<Integer>> futures = List.of(CompletableFuture.completedFuture(9));

        CompletableFuture<List<Integer>> combined = CompletableFutureAllOf.collectAll(futures);

        assertEquals(List.of(9), combined.get());
    }
}
