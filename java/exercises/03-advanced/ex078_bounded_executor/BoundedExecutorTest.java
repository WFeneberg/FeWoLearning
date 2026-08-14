package fewolearning.exercises.advanced.ex078_bounded_executor;

import java.util.concurrent.CountDownLatch;
import java.util.concurrent.CyclicBarrier;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.atomic.AtomicInteger;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class BoundedExecutorTest {

    @Test
    void submitRunsTheGivenTask() throws InterruptedException {
        ExecutorService executorService = Executors.newFixedThreadPool(2);
        BoundedExecutor bounded = new BoundedExecutor(executorService, 2);
        CountDownLatch completed = new CountDownLatch(1);

        bounded.submit(completed::countDown);

        completed.await();
        executorService.shutdown();
    }

    @Test
    void neverRunsMoreConcurrentTasksThanTheConfiguredLimit() throws Exception {
        int maxConcurrentTasks = 2;
        int taskCount = 6;
        ExecutorService executorService = Executors.newFixedThreadPool(taskCount);
        BoundedExecutor bounded = new BoundedExecutor(executorService, maxConcurrentTasks);

        AtomicInteger active = new AtomicInteger();
        AtomicInteger peak = new AtomicInteger();
        CyclicBarrier barrier = new CyclicBarrier(maxConcurrentTasks);
        CountDownLatch completed = new CountDownLatch(taskCount);

        for (int i = 0; i < taskCount; i++) {
            bounded.submit(() -> {
                int current = active.incrementAndGet();
                peak.updateAndGet(previous -> Math.max(previous, current));
                try {
                    barrier.await();
                } catch (Exception e) {
                    Thread.currentThread().interrupt();
                } finally {
                    active.decrementAndGet();
                    completed.countDown();
                }
            });
        }

        completed.await();
        executorService.shutdown();

        assertEquals(maxConcurrentTasks, peak.get());
    }
}
