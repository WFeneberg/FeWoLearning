package fewolearning.exercises.beginner.ex035_junit_assertions;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class CalculatorTest {

    @Test
    void addReturnsTheSumOfTwoNumbers() {
        // Arrange
        int left = 4;
        int right = 5;

        // Act
        int result = Calculator.add(left, right);

        // Assert
        assertEquals(9, result);
    }

    @Test
    void addHandlesNegativeNumbers() {
        assertEquals(-1, Calculator.add(-5, 4));
    }

    @Test
    void subtractReturnsTheDifferenceOfTwoNumbers() {
        // Arrange
        int left = 10;
        int right = 3;

        // Act
        int result = Calculator.subtract(left, right);

        // Assert
        assertEquals(7, result);
    }

    @Test
    void subtractCanProduceANegativeResult() {
        assertEquals(-4, Calculator.subtract(1, 5));
    }
}
