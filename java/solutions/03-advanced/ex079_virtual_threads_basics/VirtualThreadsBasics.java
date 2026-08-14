package fewolearning.exercises.advanced.ex079_virtual_threads_basics;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

/*
Exercise 079 - Virtual threads basics (reference solution).
*/
public final class VirtualThreadsBasics {
    private VirtualThreadsBasics() {
    }

    public static List<Integer> runAll(List<Callable<Integer>> tasks) throws InterruptedException {
        List<Future<Integer>> futures = new ArrayList<>();
        try (ExecutorService executor = Executors.newVirtualThreadPerTaskExecutor()) {
            for (Callable<Integer> task : tasks) {
                futures.add(executor.submit(task));
            }
            List<Integer> results = new ArrayList<>();
            for (Future<Integer> future : futures) {
                try {
                    results.add(future.get());
                } catch (ExecutionException e) {
                    throw new RuntimeException(e.getCause());
                }
            }
            return results;
        }
    }
}
