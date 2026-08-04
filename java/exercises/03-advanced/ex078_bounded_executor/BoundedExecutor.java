package fewolearning.exercises.advanced.ex078_bounded_executor;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Semaphore;

/*
Exercise 078 - Bounded executor (advanced).

Goal:   Limit in-flight tasks submitted to an executor using a semaphore.
Drills: semaphore back-pressure, pools.
*/
public final class BoundedExecutor {
    private final ExecutorService executor;
    private final Semaphore semaphore;

    public BoundedExecutor(ExecutorService executor, int maxConcurrentTasks) {
        this.executor = executor;
        this.semaphore = new Semaphore(maxConcurrentTasks);
    }

    public void submit(Runnable task) throws InterruptedException {
        throw new UnsupportedOperationException("TODO");
    }
}
