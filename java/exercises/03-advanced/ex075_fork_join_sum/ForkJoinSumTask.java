package fewolearning.exercises.advanced.ex075_fork_join_sum;

import java.util.concurrent.RecursiveTask;

/*
Exercise 075 - Fork/join sum (advanced).

Goal:   Sum a large array by splitting the work across RecursiveTask subtasks.
Drills: fork/join tasks, work splitting.
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
        throw new UnsupportedOperationException("TODO");
    }
}
