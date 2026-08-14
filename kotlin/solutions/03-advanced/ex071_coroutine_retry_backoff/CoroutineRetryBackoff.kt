package fewolearning.exercises.advanced.ex071_coroutine_retry_backoff

import kotlinx.coroutines.delay

/** Retries [operation] up to [maxAttempts] times, doubling the delay after each failed attempt. */
suspend fun <T> retryWithBackoff(maxAttempts: Int, initialDelayMillis: Long, operation: suspend () -> T): T {
    var currentDelay = initialDelayMillis
    var lastError: Throwable? = null
    repeat(maxAttempts) { attempt ->
        try {
            return operation()
        } catch (error: Exception) {
            lastError = error
            if (attempt < maxAttempts - 1) {
                delay(currentDelay)
                currentDelay *= 2
            }
        }
    }
    throw lastError ?: IllegalStateException("retryWithBackoff failed with no recorded error")
}
