package fewolearning.exercises.beginner.ex027_sequence_lazy_basics

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertNull

class SequenceLazyBasicsTest {

    @Test
    fun firstSquareAboveReturnsTheFirstSquareThatExceedsTheThreshold() {
        assertEquals(16, firstSquareAbove(listOf(1, 2, 3, 4, 5), 10))
    }

    @Test
    fun firstSquareAboveReturnsNullWhenNoSquareExceedsTheThreshold() {
        assertNull(firstSquareAbove(listOf(1, 2, 3), 100))
    }

    @Test
    fun firstSquareAboveReturnsNullForAnEmptyList() {
        assertNull(firstSquareAbove(emptyList(), 0))
    }
}
