package fewolearning.exercises.advanced.ex076_actor_counter

import kotlinx.coroutines.channels.SendChannel

/*
Exercise 076 - Actor counter (advanced).

Goal:   Serialize counter increments through a single actor-style channel.
Drills: actor model, serialized state changes.
*/
sealed class CounterMessage {
    object Increment : CounterMessage()
    class GetValue(val response: SendChannel<Int>) : CounterMessage()
}

suspend fun handleMessage(currentValue: Int, message: CounterMessage): Int {
    TODO()
}
