package fewolearning.exercises.intermediate.ex069_junit_exception_testing;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class InputValidatorTest {

    @Test
    void acceptsANonBlankValue() {
        assertDoesNotThrow(() -> InputValidator.requireNonBlank("hello", "username"));
    }

    @Test
    void rejectsANullValueWithTheFieldNameInTheMessage() {
        IllegalArgumentException thrown = assertThrows(IllegalArgumentException.class,
                () -> InputValidator.requireNonBlank(null, "username"));

        assertTrue(thrown.getMessage().contains("username"));
    }

    @Test
    void rejectsABlankValueWithTheFieldNameInTheMessage() {
        IllegalArgumentException thrown = assertThrows(IllegalArgumentException.class,
                () -> InputValidator.requireNonBlank("   ", "email"));

        assertTrue(thrown.getMessage().contains("email"));
    }
}
