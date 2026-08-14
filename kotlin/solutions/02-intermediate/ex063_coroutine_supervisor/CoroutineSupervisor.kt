package fewolearning.exercises.intermediate.ex063_coroutine_supervisor

import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.launch
import kotlinx.coroutines.supervisorScope

/** Runs each task under a supervisor so one task's failure does not cancel the others. */
suspend fun runIsolated(scope: CoroutineScope, tasks: List<suspend () -> Unit>) {
    supervisorScope {
        tasks.forEach { task ->
            launch {
                runCatching { task() }
            }
        }
    }
}
