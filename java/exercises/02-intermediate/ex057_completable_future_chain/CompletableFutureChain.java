package fewolearning.exercises.intermediate.ex057_completable_future_chain;

import java.util.concurrent.CompletableFuture;

/*
Exercise 057 - CompletableFuture chain (intermediate).

Goal:   Chain async steps that fetch, transform, and format a value.
Drills: async composition, continuations.
*/
public final class CompletableFutureChain {
    private CompletableFutureChain() {
    }

    public static CompletableFuture<String> fetchAndFormat(CompletableFuture<Integer> source) {
        throw new UnsupportedOperationException("TODO");
    }
}
