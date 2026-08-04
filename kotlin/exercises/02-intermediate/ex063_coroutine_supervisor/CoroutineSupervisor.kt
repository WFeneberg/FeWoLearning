package fewolearning.exercises.intermediate.ex063_coroutine_supervisor

import kotlinx.coroutines.CoroutineScope

/*
Exercise 063 - Coroutine supervisor (intermediate).

Goal:   Run children under a SupervisorJob so one failure does not cancel siblings.
Drills: SupervisorJob, isolated failures.
*/
suspend fun runIsolated(scope: CoroutineScope, tasks: List<suspend () -> Unit>) {
    TODO()
}
