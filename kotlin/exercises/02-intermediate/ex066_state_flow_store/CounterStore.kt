package fewolearning.exercises.intermediate.ex066_state_flow_store

import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow

/*
Exercise 066 - StateFlow store (intermediate).

Goal:   Expose a read-only StateFlow of a counter that a store can increment.
Drills: StateFlow, UI-facing state.
*/
class CounterStore {
    private val _count = MutableStateFlow(0)
    val count: StateFlow<Int> get() = _count

    fun increment() {
        TODO()
    }
}
