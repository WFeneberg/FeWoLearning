package fewolearning.exercises.intermediate.ex059_executor_service_batch;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Future;

/*
Exercise 059 - ExecutorService batch (reference solution).
*/
public final class ExecutorServiceBatch {
    private ExecutorServiceBatch() {
    }

    public static List<Integer> runAll(ExecutorService executor, List<Callable<Integer>> tasks) throws InterruptedException {
        try {
            List<Future<Integer>> futures = executor.invokeAll(tasks);
            List<Integer> results = new ArrayList<>();
            for (Future<Integer> future : futures) {
                try {
                    results.add(future.get());
                } catch (ExecutionException e) {
                    throw new RuntimeException("task failed", e.getCause());
                }
            }
            return results;
        } finally {
            executor.shutdown();
        }
    }
}
