package fewolearning.exercises.beginner.ex003_ranges_and_when

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertFalse
import org.junit.jupiter.api.Assertions.assertTrue

class RangesAndWhenTest {

    @Test
    fun classifyAssignsLetterGradesByRange() {
        assertEquals("A", classify(95))
        assertEquals("B", classify(85))
        assertEquals("C", classify(75))
        assertEquals("D", classify(65))
        assertEquals("F", classify(50))
    }

    @Test
    fun classifyHandlesBoundaryScores() {
        assertEquals("A", classify(90))
        assertEquals("B", classify(89))
    }

    @Test
    fun isInRangeChecksInclusiveBounds() {
        assertTrue(isInRange(5, 1..10))
        assertTrue(isInRange(1, 1..10))
        assertTrue(isInRange(10, 1..10))
        assertFalse(isInRange(11, 1..10))
    }
}
