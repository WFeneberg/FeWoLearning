package fewolearning.exercises.beginner.ex021_object_singleton

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ObjectSingletonTest {

    // RequestCounter is a singleton object: its count is shared across every
    // test in this class (and the whole JVM), so this test reads the current
    // value first and asserts relative to that baseline instead of assuming 0.
    @Test
    fun incrementIncreasesTheSharedCountByOneEachCall() {
        val before = RequestCounter.current()

        val afterFirst = RequestCounter.increment()
        val afterSecond = RequestCounter.increment()

        assertEquals(before + 1, afterFirst)
        assertEquals(before + 2, afterSecond)
        assertEquals(before + 2, RequestCounter.current())
    }
}
