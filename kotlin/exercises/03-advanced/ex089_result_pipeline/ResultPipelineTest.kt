package fewolearning.exercises.advanced.ex089_result_pipeline

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertTrue

class ResultPipelineTest {

    @Test
    fun parsePositiveIntSucceedsForAPositiveNumber() {
        val result = parsePositiveInt("5")

        assertEquals(5, result.getOrNull())
    }

    @Test
    fun parsePositiveIntFailsForANonPositiveNumber() {
        val result = parsePositiveInt("-3")

        assertTrue(result.isFailure)
    }

    @Test
    fun parsePositiveIntFailsForNonNumericInput() {
        val result = parsePositiveInt("abc")

        assertTrue(result.isFailure)
    }

    @Test
    fun pipelineDoublesThenIncrementsAValidValue() {
        val result = pipeline("5")

        assertEquals(11, result.getOrNull())
    }

    @Test
    fun pipelineShortCircuitsOnAnInvalidValueWithoutRunningLaterSteps() {
        val result = pipeline("-5")

        assertTrue(result.isFailure)
    }
}
