package fewolearning.exercises.advanced.ex075_fork_join_sum;

import java.util.concurrent.RecursiveTask;

/*
Exercise 075 - Fork/join sum (reference solution).
*/
public final class ForkJoinSumTask extends RecursiveTask<Long> {
    private static final int THRESHOLD = 1_000;
    private final long[] numbers;
    private final int start;
    private final int end;

    public ForkJoinSumTask(long[] numbers, int start, int end) {
        this.numbers = numbers;
        this.start = start;
        this.end = end;
    }

    @Override
    protected Long compute() {
        int length = end - start;
        if (length <= THRESHOLD) {
            long sum = 0;
            for (int i = start; i < end; i++) {
                sum += numbers[i];
            }
            return sum;
        }
        int mid = start + length / 2;
        ForkJoinSumTask left = new ForkJoinSumTask(numbers, start, mid);
        ForkJoinSumTask right = new ForkJoinSumTask(numbers, mid, end);
        left.fork();
        long rightResult = right.compute();
        long leftResult = left.join();
        return leftResult + rightResult;
    }
}
