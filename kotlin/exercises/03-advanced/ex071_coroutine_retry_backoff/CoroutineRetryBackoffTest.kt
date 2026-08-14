package fewolearning.exercises.advanced.ex071_coroutine_retry_backoff

import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertNotNull

class CoroutineRetryBackoffTest {

    @Test
    fun succeedsAfterFailingTwiceAndCallsOperationExactlyThreeTimes() = runTest {
        var callCount = 0
        val result = retryWithBackoff(maxAttempts = 5, initialDelayMillis = 100) {
            callCount += 1
            if (callCount < 3) throw RuntimeException("not yet") else "ok"
        }

        assertEquals("ok", result)
        assertEquals(3, callCount)
    }

    @Test
    fun throwsAfterExhaustingAllAttemptsHavingCalledTheOperationMaxAttemptsTimes() = runTest {
        var callCount = 0
        var caught: Throwable? = null

        try {
            retryWithBackoff(maxAttempts = 3, initialDelayMillis = 50) {
                callCount += 1
                throw RuntimeException("always fails")
            }
        } catch (error: RuntimeException) {
            caught = error
        }

        assertNotNull(caught)
        assertEquals(3, callCount)
    }
}
