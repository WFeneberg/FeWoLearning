package fewolearning.exercises.beginner.ex008_data_class_point

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class DataClassPointTest {

    @Test
    fun translateShiftsBothCoordinates() {
        assertEquals(Point(4, 6), translate(Point(1, 2), 3, 4))
    }

    @Test
    fun translateSupportsNegativeDeltas() {
        assertEquals(Point(-1, -1), translate(Point(0, 0), -1, -1))
    }

    @Test
    fun translateDoesNotMutateTheOriginalPoint() {
        val original = Point(1, 1)
        translate(original, 5, 5)
        assertEquals(Point(1, 1), original)
    }
}
