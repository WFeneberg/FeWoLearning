package fewolearning.exercises.intermediate.ex059_executor_service_batch;

import java.util.List;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;

/*
Exercise 059 - ExecutorService batch (intermediate).

Goal:   Submit a batch of tasks to an ExecutorService and shut it down cleanly.
Drills: task submission, shutdown.
*/
public final class ExecutorServiceBatch {
    private ExecutorServiceBatch() {
    }

    public static List<Integer> runAll(ExecutorService executor, List<Callable<Integer>> tasks) throws InterruptedException {
        throw new UnsupportedOperationException("TODO");
    }
}
