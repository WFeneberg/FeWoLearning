package fewolearning.exercises.advanced.ex084_lazy_thread_safety

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertSame

class SynchronizedLazyConfigTest {

    @Test
    fun theLoaderRunsExactlyOnceAndSubsequentReadsReturnTheSameCachedInstance() {
        var invocationCount = 0
        val config = SynchronizedLazyConfig {
            invocationCount += 1
            "loaded-value"
        }

        assertEquals(0, invocationCount)

        val first = config.value
        val second = config.value

        assertEquals("loaded-value", first)
        assertSame(first, second)
        assertEquals(1, invocationCount)
    }
}
