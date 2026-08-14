package fewolearning.exercises.intermediate.ex057_completable_future_chain;

import java.util.concurrent.CompletableFuture;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class CompletableFutureChainTest {

    @Test
    void fetchAndFormatDoublesTheValueAndFormatsTheResult() throws Exception {
        CompletableFuture<Integer> source = CompletableFuture.completedFuture(21);

        CompletableFuture<String> result = CompletableFutureChain.fetchAndFormat(source);

        assertEquals("Result: 42", result.get());
    }

    @Test
    void fetchAndFormatHandlesZero() throws Exception {
        CompletableFuture<Integer> source = CompletableFuture.completedFuture(0);

        CompletableFuture<String> result = CompletableFutureChain.fetchAndFormat(source);

        assertEquals("Result: 0", result.get());
    }
}
