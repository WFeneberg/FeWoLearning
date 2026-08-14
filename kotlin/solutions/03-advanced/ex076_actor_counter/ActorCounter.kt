package fewolearning.exercises.advanced.ex076_actor_counter

import kotlinx.coroutines.channels.SendChannel

sealed class CounterMessage {
    object Increment : CounterMessage()
    class GetValue(val response: SendChannel<Int>) : CounterMessage()
}

/** Pure per-message state transition: computes the next counter state for a single message. */
suspend fun handleMessage(currentValue: Int, message: CounterMessage): Int =
    when (message) {
        is CounterMessage.Increment -> currentValue + 1
        is CounterMessage.GetValue -> {
            message.response.send(currentValue)
            currentValue
        }
    }
