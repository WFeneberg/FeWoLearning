package fewolearning.exercises.intermediate.ex062_atomic_integer_counter;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class AtomicIntegerCounterTest {

    @Test
    void incrementReturnsTheUpdatedValue() {
        AtomicIntegerCounter counter = new AtomicIntegerCounter();

        assertEquals(1, counter.increment());
        assertEquals(2, counter.increment());
    }

    @Test
    void resetReturnsTheCounterToZero() {
        AtomicIntegerCounter counter = new AtomicIntegerCounter();
        counter.increment();

        counter.reset();

        assertEquals(0, counter.current());
    }

    @Test
    void survivesConcurrentIncrementsFromManyThreads() throws InterruptedException {
        AtomicIntegerCounter counter = new AtomicIntegerCounter();
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
