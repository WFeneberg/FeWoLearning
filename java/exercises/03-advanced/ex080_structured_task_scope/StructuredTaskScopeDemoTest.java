package fewolearning.exercises.advanced.ex080_structured_task_scope;

import java.util.concurrent.CountDownLatch;
import java.util.concurrent.ExecutionException;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class StructuredTaskScopeDemoTest {

    @Test
    void returnsTheValueOfTheTaskThatSucceeds() throws Exception {
        String result = StructuredTaskScopeDemo.runFirstSuccessful(
                () -> "fast",
                () -> {
                    throw new IllegalStateException("slow task fails");
                }
        );

        assertEquals("fast", result);
    }

    @Test
    void returnsTheSecondTasksValueWhenTheFirstFails() throws Exception {
        String result = StructuredTaskScopeDemo.runFirstSuccessful(
                () -> {
                    throw new IllegalStateException("first task fails");
                },
                () -> "second"
        );

        assertEquals("second", result);
    }

    @Test
    void propagatesAFailureWhenBothTasksFail() {
        assertThrows(ExecutionException.class, () -> StructuredTaskScopeDemo.runFirstSuccessful(
                () -> {
                    throw new IllegalStateException("first fails");
                },
                () -> {
                    throw new IllegalStateException("second fails");
                }
        ));
    }

    @Test
    void cancelsTheSiblingTaskOnceAWinnerIsFound() throws Exception {
        CountDownLatch siblingInterrupted = new CountDownLatch(1);

        String result = StructuredTaskScopeDemo.runFirstSuccessful(
                () -> "winner",
                () -> {
                    try {
                        Thread.sleep(60_000);
                    } catch (InterruptedException e) {
                        siblingInterrupted.countDown();
                        throw e;
                    }
                    return "loser";
                }
        );

        siblingInterrupted.await();
        assertEquals("winner", result);
    }
}
