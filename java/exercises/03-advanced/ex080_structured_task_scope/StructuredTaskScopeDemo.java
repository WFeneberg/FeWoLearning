package fewolearning.exercises.advanced.ex080_structured_task_scope;

import java.util.concurrent.Callable;

/*
Exercise 080 - Structured task scope (advanced).

Goal:   Run two subtasks with structured concurrency, cancelling the sibling on failure.
Drills: structured concurrency, cancellation.
*/
public final class StructuredTaskScopeDemo {
    private StructuredTaskScopeDemo() {
    }

    public static <T> T runFirstSuccessful(Callable<T> first, Callable<T> second) throws Exception {
        throw new UnsupportedOperationException("TODO");
    }
}
