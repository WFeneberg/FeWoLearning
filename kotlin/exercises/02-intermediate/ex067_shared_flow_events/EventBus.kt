package fewolearning.exercises.intermediate.ex067_shared_flow_events

import kotlinx.coroutines.flow.MutableSharedFlow
import kotlinx.coroutines.flow.SharedFlow

/*
Exercise 067 - SharedFlow events (intermediate).

Goal:   Broadcast one-off events to all current subscribers via a SharedFlow.
Drills: SharedFlow, broadcast events.
*/
class EventBus {
    private val _events = MutableSharedFlow<String>()
    val events: SharedFlow<String> get() = _events

    suspend fun publish(event: String) {
        TODO()
    }
}
