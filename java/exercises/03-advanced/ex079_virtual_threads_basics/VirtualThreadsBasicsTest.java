package fewolearning.exercises.advanced.ex079_virtual_threads_basics;

import java.util.List;
import java.util.concurrent.Callable;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertInstanceOf;
import static org.junit.jupiter.api.Assertions.assertThrows;

class VirtualThreadsBasicsTest {

    @Test
    void runsAllTasksAndReturnsResultsInSubmissionOrder() throws InterruptedException {
        List<Callable<Integer>> tasks = List.of(
                () -> 1,
                () -> 2,
                () -> 3
        );

        List<Integer> results = VirtualThreadsBasics.runAll(tasks);

        assertEquals(List.of(1, 2, 3), results);
    }

    @Test
    void runsABatchOfManyTasksConcurrentlyOnVirtualThreads() throws InterruptedException {
        List<Callable<Integer>> tasks = List.of(
                () -> doubleValue(1),
                () -> doubleValue(2),
                () -> doubleValue(3),
                () -> doubleValue(4)
        );

        List<Integer> results = VirtualThreadsBasics.runAll(tasks);

        assertEquals(List.of(2, 4, 6, 8), results);
    }

    @Test
    void wrapsAFailingTaskAsAnUncheckedException() {
        List<Callable<Integer>> tasks = List.of(() -> {
            throw new IllegalStateException("boom");
        });

        RuntimeException thrown = assertThrows(RuntimeException.class, () -> VirtualThreadsBasics.runAll(tasks));
        IllegalStateException cause = assertInstanceOf(IllegalStateException.class, thrown.getCause());
        assertEquals("boom", cause.getMessage());
    }

    private static int doubleValue(int value) {
        return value * 2;
    }
}
