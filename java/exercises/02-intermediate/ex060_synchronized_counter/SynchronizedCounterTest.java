package fewolearning.exercises.intermediate.ex060_synchronized_counter;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class SynchronizedCounterTest {

    @Test
    void incrementIncreasesTheCountByOne() {
        SynchronizedCounter counter = new SynchronizedCounter();

        counter.increment();
        counter.increment();

        assertEquals(2, counter.current());
    }

    @Test
    void survivesConcurrentIncrementsFromManyThreads() throws InterruptedException {
        SynchronizedCounter counter = new SynchronizedCounter();
        int threadCount = 8;
        int incrementsPerThread = 2000;

        Thread[] threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++) {
            threads[i] = new Thread(() -> {
                for (int j = 0; j < incrementsPerThread; j++) {
                    counter.increment();
                }
            });
        }
        for (Thread thread : threads) {
            thread.start();
        }
        for (Thread thread : threads) {
            thread.join();
        }

        assertEquals(threadCount * incrementsPerThread, counter.current());
    }
}
