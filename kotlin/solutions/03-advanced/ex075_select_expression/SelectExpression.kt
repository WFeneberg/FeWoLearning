package fewolearning.exercises.advanced.ex075_select_expression

import kotlinx.coroutines.channels.ReceiveChannel
import kotlinx.coroutines.selects.select

/** Suspends until either channel produces a value, returning whichever arrives first. */
suspend fun <T> firstAvailable(first: ReceiveChannel<T>, second: ReceiveChannel<T>): T =
    select {
        first.onReceive { it }
        second.onReceive { it }
    }
