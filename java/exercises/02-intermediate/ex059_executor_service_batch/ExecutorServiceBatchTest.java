package fewolearning.exercises.intermediate.ex059_executor_service_batch;

import java.util.List;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ExecutorServiceBatchTest {

    @Test
    void runAllReturnsEachTasksResultInOrder() throws InterruptedException {
        ExecutorService executor = Executors.newFixedThreadPool(2);
        List<Callable<Integer>> tasks = List.of(() -> 1, () -> 2, () -> 3);

        List<Integer> results = ExecutorServiceBatch.runAll(executor, tasks);

        assertEquals(List.of(1, 2, 3), results);
    }

    @Test
    void runAllShutsDownTheExecutorAfterwards() throws InterruptedException {
        ExecutorService executor = Executors.newFixedThreadPool(2);
        List<Callable<Integer>> tasks = List.of(() -> 1, () -> 2);

        ExecutorServiceBatch.runAll(executor, tasks);

        assertTrue(executor.isShutdown());
    }
}
