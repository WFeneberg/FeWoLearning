package fewolearning.exercises.intermediate.ex066_state_flow_store

import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow

/** Exposes a read-only StateFlow of a counter that this store can increment. */
class CounterStore {
    private val _count = MutableStateFlow(0)
    val count: StateFlow<Int> get() = _count

    fun increment() {
        _count.value += 1
    }
}
