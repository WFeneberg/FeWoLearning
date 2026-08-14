package fewolearning.exercises.beginner.ex032_junit_assertions

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class CalculatorTest {

    @Test
    fun addReturnsTheSumOfTwoNumbers() {
        // Arrange
        val left = 2
        val right = 3

        // Act
        val result = add(left, right)

        // Assert
        assertEquals(5, result)
    }

    @Test
    fun subtractReturnsTheDifferenceOfTwoNumbers() {
        // Arrange
        val left = 10
        val right = 4

        // Act
        val result = subtract(left, right)

        // Assert
        assertEquals(6, result)
    }

    @Test
    fun subtractCanReturnANegativeResult() {
        // Arrange
        val left = 3
        val right = 10

        // Act
        val result = subtract(left, right)

        // Assert
        assertEquals(-7, result)
    }
}
