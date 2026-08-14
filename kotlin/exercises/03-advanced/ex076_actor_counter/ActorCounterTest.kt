package fewolearning.exercises.advanced.ex076_actor_counter

import kotlinx.coroutines.channels.Channel
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ActorCounterTest {

    @Test
    fun incrementReturnsTheNextValueWithoutAnySideEffect() = runTest {
        val next = handleMessage(5, CounterMessage.Increment)

        assertEquals(6, next)
    }

    @Test
    fun getValueSendsTheCurrentValueAndLeavesItUnchanged() = runTest {
        val response = Channel<Int>(capacity = 1)

        val stateAfterGet = handleMessage(42, CounterMessage.GetValue(response))

        assertEquals(42, response.receive())
        assertEquals(42, stateAfterGet)
    }
}
