package fewolearning.exercises.advanced.ex079_virtual_threads_basics;

import java.util.List;
import java.util.concurrent.Callable;

/*
Exercise 079 - Virtual threads basics (advanced).

Goal:   Run a batch of blocking tasks concurrently using a virtual-thread-per-task executor.
Drills: virtual threads, blocking style concurrency.
*/
public final class VirtualThreadsBasics {
    private VirtualThreadsBasics() {
    }

    public static List<Integer> runAll(List<Callable<Integer>> tasks) throws InterruptedException {
        throw new UnsupportedOperationException("TODO");
    }
}
