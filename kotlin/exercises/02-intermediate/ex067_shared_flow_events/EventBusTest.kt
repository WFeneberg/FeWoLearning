package fewolearning.exercises.intermediate.ex067_shared_flow_events

import kotlinx.coroutines.flow.collect
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.launch
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class EventBusTest {

    @Test
    fun publishBroadcastsToActiveSubscribers() = runTest {
        val bus = EventBus()
        val received = mutableListOf<String>()
        val job = launch { bus.events.collect { received.add(it) } }
        bus.events.subscriptionCount.first { it > 0 }

        bus.publish("hello")

        assertEquals(listOf("hello"), received)
        job.cancel()
    }
}
