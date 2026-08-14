package fewolearning.exercises.intermediate.ex058_completable_future_allof;

import java.util.List;
import java.util.concurrent.CompletableFuture;
import java.util.stream.Collectors;

/*
Exercise 058 - CompletableFuture allOf (reference solution).
*/
public final class CompletableFutureAllOf {
    private CompletableFutureAllOf() {
    }

    public static CompletableFuture<List<Integer>> collectAll(List<CompletableFuture<Integer>> futures) {
        CompletableFuture<Void> allDone = CompletableFuture.allOf(futures.toArray(new CompletableFuture[0]));
        return allDone.thenApply(ignored -> futures.stream()
                .map(CompletableFuture::join)
                .collect(Collectors.toList()));
    }
}
