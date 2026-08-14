package fewolearning.exercises.intermediate.ex049_optional_pipeline;

import java.util.Map;
import java.util.Optional;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

class OptionalPipelineTest {

    @Test
    void lookupUpperCaseReturnsTheUpperCasedValueWhenPresent() {
        Map<String, String> source = Map.of("greeting", "hello");

        Optional<String> result = OptionalPipeline.lookupUpperCase(source, "greeting");

        assertTrue(result.isPresent());
        assertEquals("HELLO", result.get());
    }

    @Test
    void lookupUpperCaseReturnsEmptyWhenTheKeyIsMissing() {
        Map<String, String> source = Map.of("greeting", "hello");

        Optional<String> result = OptionalPipeline.lookupUpperCase(source, "missing");

        assertFalse(result.isPresent());
    }

    @Test
    void resolveOrComputeReturnsThePresentValueWithoutUsingTheFallback() {
        String resolved = OptionalPipeline.resolveOrCompute(Optional.of("value"), () -> {
            throw new AssertionError("fallback should not run");
        });

        assertEquals("value", resolved);
    }

    @Test
    void resolveOrComputeUsesTheFallbackWhenEmpty() {
        String resolved = OptionalPipeline.resolveOrCompute(Optional.empty(), () -> "computed");

        assertEquals("computed", resolved);
    }
}
