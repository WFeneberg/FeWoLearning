package fewolearning.exercises.intermediate.ex049_optional_pipeline;

import java.util.Map;
import java.util.Optional;
import java.util.function.Supplier;

/*
Exercise 049 - Optional pipeline (intermediate).

Goal:   Chain lookups through a pipeline of Optionals without null checks.
Drills: map, flatMap, orElseGet.
*/
public final class OptionalPipeline {
    private OptionalPipeline() {
    }

    public static Optional<String> lookupUpperCase(Map<String, String> source, String key) {
        throw new UnsupportedOperationException("TODO");
    }

    public static String resolveOrCompute(Optional<String> value, Supplier<String> fallback) {
        throw new UnsupportedOperationException("TODO");
    }
}
