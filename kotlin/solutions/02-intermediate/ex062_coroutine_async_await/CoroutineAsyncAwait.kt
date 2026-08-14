package fewolearning.exercises.intermediate.ex062_coroutine_async_await

import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.async

/** Runs [first] and [second] concurrently via async/await and sums their results. */
suspend fun sumConcurrently(scope: CoroutineScope, first: suspend () -> Int, second: suspend () -> Int): Int {
    val firstDeferred = scope.async { first() }
    val secondDeferred = scope.async { second() }
    return firstDeferred.await() + secondDeferred.await()
}
