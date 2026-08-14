package fewolearning.exercises.beginner.ex028_custom_exception;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class CustomExceptionTest {

    @Test
    void requirePositiveDoesNothingForAPositiveValue() {
        assertDoesNotThrow(() -> CustomException.requirePositive(5));
    }

    @Test
    void requirePositiveThrowsWithAClearMessageForZero() {
        CustomException.InvalidAmountException exception = assertThrows(
                CustomException.InvalidAmountException.class,
                () -> CustomException.requirePositive(0));

        assertEquals("amount must be positive: 0", exception.getMessage());
    }

    @Test
    void requirePositiveThrowsForANegativeValue() {
        CustomException.InvalidAmountException exception = assertThrows(
                CustomException.InvalidAmountException.class,
                () -> CustomException.requirePositive(-3));

        assertEquals("amount must be positive: -3", exception.getMessage());
    }
}
