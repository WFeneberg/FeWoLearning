package fewolearning.exercises.intermediate.ex066_state_flow_store

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class CounterStoreTest {

    @Test
    fun incrementUpdatesTheExposedStateFlowValue() {
        val store = CounterStore()

        assertEquals(0, store.count.value)

        store.increment()
        store.increment()

        assertEquals(2, store.count.value)
    }
}
