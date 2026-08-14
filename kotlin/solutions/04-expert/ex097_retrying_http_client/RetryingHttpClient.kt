package fewolearning.exercises.expert.ex097_retrying_http_client

/**
 * Calls delegate.get up to maxAttempts total attempts (no backoff delay - not required by
 * the stub), returning the first success and rethrowing the last failure if every attempt fails.
 */
interface HttpClientFacade {
    suspend fun get(url: String): String
}

class RetryingHttpClient(private val delegate: HttpClientFacade, private val maxAttempts: Int) {
    suspend fun get(url: String): String {
        var lastError: Throwable? = null
        repeat(maxAttempts) {
            try {
                return delegate.get(url)
            } catch (e: Exception) {
                lastError = e
            }
        }
        throw lastError ?: IllegalStateException("maxAttempts must be at least 1")
    }
}
