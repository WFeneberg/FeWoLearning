package fewolearning.exercises.advanced.ex078_bounded_executor;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Semaphore;

/*
Exercise 078 - Bounded executor (reference solution).
*/
public final class BoundedExecutor {
    private final ExecutorService executor;
    private final Semaphore semaphore;

    public BoundedExecutor(ExecutorService executor, int maxConcurrentTasks) {
        this.executor = executor;
        this.semaphore = new Semaphore(maxConcurrentTasks);
    }

    public void submit(Runnable task) throws InterruptedException {
        semaphore.acquire();
        executor.submit(() -> {
            try {
                task.run();
            } finally {
                semaphore.release();
            }
        });
    }
}
