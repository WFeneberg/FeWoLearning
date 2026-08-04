package fewolearning.exercises.intermediate.ex058_completable_future_allof;

import java.util.List;
import java.util.concurrent.CompletableFuture;

/*
Exercise 058 - CompletableFuture allOf (intermediate).

Goal:   Wait for a batch of futures to complete and collect their results.
Drills: waiting for many tasks.
*/
public final class CompletableFutureAllOf {
    private CompletableFutureAllOf() {
    }

    public static CompletableFuture<List<Integer>> collectAll(List<CompletableFuture<Integer>> futures) {
        throw new UnsupportedOperationException("TODO");
    }
}
