package fewolearning.exercises.intermediate.ex061_reentrant_lock_guard;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class ReentrantLockGuardTest {

    @Test
    void incrementIncreasesTheCountByOne() {
        ReentrantLockGuard guard = new ReentrantLockGuard();

        guard.increment();
        guard.increment();

        assertEquals(2, guard.current());
    }

    @Test
    void survivesConcurrentIncrementsFromManyThreads() throws InterruptedException {
        ReentrantLockGuard guard = new ReentrantLockGuard();
        int threadCount = 8;
        int incrementsPerThread = 2000;

        Thread[] threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++) {
            threads[i] = new Thread(() -> {
                for (int j = 0; j < incrementsPerThread; j++) {
                    guard.increment();
                }
            });
        }
        for (Thread thread : threads) {
            thread.start();
        }
        for (Thread thread : threads) {
            thread.join();
        }

        assertEquals(threadCount * incrementsPerThread, guard.current());
    }
}
