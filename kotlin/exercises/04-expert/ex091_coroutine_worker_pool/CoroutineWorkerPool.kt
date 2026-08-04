package fewolearning.exercises.expert.ex091_coroutine_worker_pool

import kotlinx.coroutines.channels.ReceiveChannel

/*
Exercise 091 - Coroutine worker pool (expert).

Goal:   Process tasks from a channel using a bounded number of concurrent workers.
Drills: bounded worker pools, graceful shutdown.
*/
suspend fun processAll(tasks: ReceiveChannel<suspend () -> Unit>, workerCount: Int) {
    TODO()
}
