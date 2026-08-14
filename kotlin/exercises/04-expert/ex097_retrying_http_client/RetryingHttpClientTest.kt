package fewolearning.exercises.expert.ex097_retrying_http_client

import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.fail

class RetryingHttpClientTest {

    private class FlakyClient(private val failuresBeforeSuccess: Int) : HttpClientFacade {
        var attempts = 0
            private set

        override suspend fun get(url: String): String {
            attempts++
            if (attempts <= failuresBeforeSuccess) throw RuntimeException("attempt $attempts failed")
            return "ok:$url"
        }
    }

    private class AlwaysFailingClient : HttpClientFacade {
        var attempts = 0
            private set

        override suspend fun get(url: String): String {
            attempts++
            throw RuntimeException("attempt $attempts failed")
        }
    }

    @Test
    fun succeedsAfterRetryingPastTransientFailures() = runTest {
        val flaky = FlakyClient(failuresBeforeSuccess = 2)
        val client = RetryingHttpClient(flaky, maxAttempts = 3)

        val result = client.get("/status")

        assertEquals("ok:/status", result)
        // Proves retrying actually happened, not just that a single call happened to succeed.
        assertEquals(3, flaky.attempts)
    }

    @Test
    fun givesUpAndPropagatesTheLastFailureAfterMaxAttempts() = runTest {
        val alwaysFails = AlwaysFailingClient()
        val client = RetryingHttpClient(alwaysFails, maxAttempts = 3)

        try {
            client.get("/status")
            fail("expected the retrying client to eventually rethrow")
        } catch (e: RuntimeException) {
            assertEquals("attempt 3 failed", e.message)
        }
        assertEquals(3, alwaysFails.attempts)
    }
}
