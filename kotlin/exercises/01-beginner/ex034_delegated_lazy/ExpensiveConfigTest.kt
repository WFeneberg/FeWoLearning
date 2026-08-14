package fewolearning.exercises.beginner.ex034_delegated_lazy

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ExpensiveConfigTest {

    @Test
    fun valueInvokesTheLoaderExactlyOnceEvenWhenReadMultipleTimes() {
        var invocationCount = 0
        val config = ExpensiveConfig {
            invocationCount += 1
            "loaded"
        }

        assertEquals(0, invocationCount)

        assertEquals("loaded", config.value)
        assertEquals("loaded", config.value)
        assertEquals("loaded", config.value)

        assertEquals(1, invocationCount)
    }
}
