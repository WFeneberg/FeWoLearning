package fewolearning.exercises.intermediate.ex067_shared_flow_events

import kotlinx.coroutines.flow.MutableSharedFlow
import kotlinx.coroutines.flow.SharedFlow

/** Broadcasts one-off events to every subscriber currently collecting [events]. */
class EventBus {
    private val _events = MutableSharedFlow<String>()
    val events: SharedFlow<String> get() = _events

    suspend fun publish(event: String) {
        _events.emit(event)
    }
}
