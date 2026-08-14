package fewolearning.exercises.intermediate.ex062_coroutine_async_await

import kotlinx.coroutines.delay
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class CoroutineAsyncAwaitTest {

    @Test
    fun sumConcurrentlyCombinesBothResultsAfterRunningThemConcurrently() = runTest {
        val result = sumConcurrently(
            this,
            first = {
                delay(100)
                10
            },
            second = {
                delay(50)
                20
            }
        )

        assertEquals(30, result)
    }
}
