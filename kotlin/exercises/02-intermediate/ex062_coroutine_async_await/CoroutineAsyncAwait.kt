package fewolearning.exercises.intermediate.ex062_coroutine_async_await

import kotlinx.coroutines.CoroutineScope

/*
Exercise 062 - Coroutine async/await (intermediate).

Goal:   Run two suspending computations concurrently and combine their results.
Drills: async, await, concurrency.
*/
suspend fun sumConcurrently(scope: CoroutineScope, first: suspend () -> Int, second: suspend () -> Int): Int {
    TODO()
}
