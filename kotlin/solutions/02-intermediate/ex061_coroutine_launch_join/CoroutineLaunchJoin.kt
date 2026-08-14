package fewolearning.exercises.intermediate.ex061_coroutine_launch_join

import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.launch

/** Launches [first] and [second] as child coroutines and waits for both to finish. */
suspend fun runBothAndWait(scope: CoroutineScope, first: suspend () -> Unit, second: suspend () -> Unit) {
    val firstJob = scope.launch { first() }
    val secondJob = scope.launch { second() }
    firstJob.join()
    secondJob.join()
}
