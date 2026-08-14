package fewolearning.exercises.intermediate.ex040_sequence_pipeline

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class SequencePipelineTest {

    @Test
    fun takesOnlyTheRequestedNumberOfEvenSquares() {
        val numbers = listOf(1, 2, 3, 4, 5, 6, 7, 8)

        val result = firstEvenSquares(numbers, 3)

        assertEquals(listOf(4, 16, 36), result)
    }

    @Test
    fun returnsFewerResultsWhenNotEnoughEvenNumbersExist() {
        val numbers = listOf(1, 2, 3)

        val result = firstEvenSquares(numbers, 5)

        assertEquals(listOf(4), result)
    }
}
