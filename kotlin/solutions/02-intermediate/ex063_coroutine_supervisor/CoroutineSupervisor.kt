package fewolearning.exercises.intermediate.ex063_coroutine_supervisor

import kotlinx.coroutines.CoroutineExceptionHandler
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.launch
import kotlinx.coroutines.supervisorScope

/**
 * Runs each task under a supervisor so one task's failure does not cancel the others.
 *
 * Each task is launched as a direct child of [supervisorScope] (not wrapped in a
 * `runCatching`/try-catch) - it is `supervisorScope`'s own isolation semantics that
 * keep a failing sibling from cancelling the rest, which is the entire point of this
 * exercise. A [CoroutineExceptionHandler] is attached only so the (expected, isolated)
 * failure doesn't escape as an uncaught exception - it does not do the isolating.
 */
suspend fun runIsolated(scope: CoroutineScope, tasks: List<suspend () -> Unit>) {
    val handler = CoroutineExceptionHandler { _, _ -> /* isolated failure: intentionally ignored */ }
    supervisorScope {
        tasks.forEach { task ->
            launch(handler) {
                task()
            }
        }
    }
}
