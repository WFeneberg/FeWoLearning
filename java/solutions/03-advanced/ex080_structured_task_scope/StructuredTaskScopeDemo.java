package fewolearning.exercises.advanced.ex080_structured_task_scope;

import java.util.List;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

/*
Exercise 080 - Structured task scope (reference solution).

Note: java.util.concurrent.StructuredTaskScope is a PREVIEW API in JDK 21 (it would
require --enable-preview, which this project's build.gradle does not configure, and
its exact shape has changed across preview iterations). ExecutorService.invokeAny is
used instead: it mirrors the same structured-concurrency semantics - run both tasks,
return the first to succeed, and cancel the rest - using a stable stdlib primitive.
*/
public final class StructuredTaskScopeDemo {
    private StructuredTaskScopeDemo() {
    }

    public static <T> T runFirstSuccessful(Callable<T> first, Callable<T> second) throws Exception {
        ExecutorService executor = Executors.newFixedThreadPool(2);
        try {
            return executor.invokeAny(List.of(first, second));
        } finally {
            executor.shutdown();
        }
    }
}
