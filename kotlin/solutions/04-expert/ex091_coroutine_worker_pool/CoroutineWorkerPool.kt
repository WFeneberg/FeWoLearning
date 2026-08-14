package fewolearning.exercises.expert.ex091_coroutine_worker_pool

import kotlinx.coroutines.channels.ReceiveChannel
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.launch

/**
 * Launches [workerCount] concurrent coroutines, each draining [tasks] with a plain
 * `for (task in tasks)` loop. The channel must be closed by the caller once every task
 * has been sent so those loops terminate naturally; `coroutineScope` then waits for all
 * workers to finish before returning.
 */
suspend fun processAll(tasks: ReceiveChannel<suspend () -> Unit>, workerCount: Int) {
    coroutineScope {
        repeat(workerCount) {
            launch {
                for (task in tasks) {
                    task()
                }
            }
        }
    }
}
