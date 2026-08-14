package fewolearning.exercises.beginner.ex017_lambda_capture

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class LambdaCaptureTest {

    @Test
    fun counterIncrementsItsCapturedStateOnEachCall() {
        val counter = makeCounter()

        assertEquals(1, counter())
        assertEquals(2, counter())
        assertEquals(3, counter())
    }

    @Test
    fun eachCounterInstanceHasItsOwnIndependentState() {
        val first = makeCounter()
        val second = makeCounter()

        first()
        first()

        assertEquals(1, second())
        assertEquals(3, first())
    }
}
