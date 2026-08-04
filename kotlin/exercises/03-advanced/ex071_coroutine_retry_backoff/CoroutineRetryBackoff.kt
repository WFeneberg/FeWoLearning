package fewolearning.exercises.advanced.ex071_coroutine_retry_backoff

/*
Exercise 071 - Coroutine retry with backoff (advanced).

Goal:   Retry a suspending operation with increasing delay between attempts.
Drills: retry loops, delay policy.
*/
suspend fun <T> retryWithBackoff(maxAttempts: Int, initialDelayMillis: Long, operation: suspend () -> T): T {
    TODO()
}
