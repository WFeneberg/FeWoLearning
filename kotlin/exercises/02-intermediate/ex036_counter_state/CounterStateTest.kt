package fewolearning.exercises.intermediate.ex036_counter_state

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class CounterStateTest {

    @Test
    fun incrementIncreasesTheCountByOne() {
        val counter = CounterState()

        assertEquals(1, counter.increment())
        assertEquals(2, counter.increment())
        assertEquals(2, counter.current())
    }

    @Test
    fun decrementNeverDropsBelowZero() {
        val counter = CounterState()

        assertEquals(0, counter.decrement())

        counter.increment()

        assertEquals(0, counter.decrement())
    }

    @Test
    fun resetSetsTheCountBackToZero() {
        val counter = CounterState()
        counter.increment()
        counter.increment()

        counter.reset()

        assertEquals(0, counter.current())
    }
}
