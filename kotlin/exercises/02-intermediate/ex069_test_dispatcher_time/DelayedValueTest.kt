package fewolearning.exercises.intermediate.ex069_test_dispatcher_time

import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class DelayedValueTest {

    @Test
    fun delayedValueResolvesInstantlyUnderVirtualTime() = runTest {
        val before = testScheduler.currentTime

        val result = delayedValue(10_000, 42)

        assertEquals(42, result)
        assertEquals(before + 10_000, testScheduler.currentTime)
    }
}
