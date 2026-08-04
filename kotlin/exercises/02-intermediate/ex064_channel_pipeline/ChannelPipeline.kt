package fewolearning.exercises.intermediate.ex064_channel_pipeline

import kotlinx.coroutines.channels.Channel

/*
Exercise 064 - Channel pipeline (intermediate).

Goal:   Send a list of values through a channel and collect them on the receiving side.
Drills: channels, producer/consumer.
*/
suspend fun pipeThrough(values: List<Int>, channel: Channel<Int>): List<Int> {
    TODO()
}
