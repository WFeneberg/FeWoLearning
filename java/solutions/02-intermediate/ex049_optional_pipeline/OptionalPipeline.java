package fewolearning.exercises.intermediate.ex049_optional_pipeline;

import java.util.Map;
import java.util.Optional;
import java.util.function.Supplier;

/*
Exercise 049 - Optional pipeline (reference solution).
*/
public final class OptionalPipeline {
    private OptionalPipeline() {
    }

    public static Optional<String> lookupUpperCase(Map<String, String> source, String key) {
        return Optional.ofNullable(source.get(key)).map(String::toUpperCase);
    }

    public static String resolveOrCompute(Optional<String> value, Supplier<String> fallback) {
        return value.orElseGet(fallback);
    }
}
