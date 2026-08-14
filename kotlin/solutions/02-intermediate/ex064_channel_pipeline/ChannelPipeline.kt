package fewolearning.exercises.intermediate.ex064_channel_pipeline

import kotlinx.coroutines.channels.Channel
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.launch

/** Sends [values] through [channel] and collects them concurrently on the receiving side. */
suspend fun pipeThrough(values: List<Int>, channel: Channel<Int>): List<Int> = coroutineScope {
    launch {
        for (v in values) channel.send(v)
        channel.close()
    }

    val received = mutableListOf<Int>()
    for (v in channel) received.add(v)
    received
}
