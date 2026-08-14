package fewolearning.exercises.intermediate.ex057_completable_future_chain;

import java.util.concurrent.CompletableFuture;

/*
Exercise 057 - CompletableFuture chain (reference solution).
*/
public final class CompletableFutureChain {
    private CompletableFutureChain() {
    }

    public static CompletableFuture<String> fetchAndFormat(CompletableFuture<Integer> source) {
        return source.thenApply(value -> value * 2)
                .thenApply(doubled -> "Result: " + doubled);
    }
}
