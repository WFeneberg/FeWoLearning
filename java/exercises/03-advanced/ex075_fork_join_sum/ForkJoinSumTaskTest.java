package fewolearning.exercises.advanced.ex075_fork_join_sum;

import java.util.concurrent.ForkJoinPool;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class ForkJoinSumTaskTest {

    @Test
    void sumsASmallArrayThatNeverSplits() {
        long[] numbers = {1, 2, 3, 4, 5};

        long result = ForkJoinPool.commonPool().invoke(new ForkJoinSumTask(numbers, 0, numbers.length));

        assertEquals(15L, result);
    }

    @Test
    void sumsALargeArrayThatRequiresSplittingAcrossSubtasks() {
        int size = 50_000;
        long[] numbers = new long[size];
        long expected = 0;
        for (int i = 0; i < size; i++) {
            numbers[i] = i + 1;
            expected += numbers[i];
        }

        long result = ForkJoinPool.commonPool().invoke(new ForkJoinSumTask(numbers, 0, numbers.length));

        assertEquals(expected, result);
    }

    @Test
    void sumsAnEmptyRangeAsZero() {
        long[] numbers = {1, 2, 3};

        long result = ForkJoinPool.commonPool().invoke(new ForkJoinSumTask(numbers, 1, 1));

        assertEquals(0L, result);
    }
}
