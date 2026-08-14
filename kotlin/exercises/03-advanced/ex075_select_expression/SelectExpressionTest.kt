package fewolearning.exercises.advanced.ex075_select_expression

import kotlinx.coroutines.channels.Channel
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class SelectExpressionTest {

    @Test
    fun returnsTheValueFromWhicheverChannelReceivesFirstWhenOnlyOneSends() = runTest {
        val first = Channel<String>()
        val second = Channel<String>()
        launch { first.send("from-first") }

        val result = firstAvailable(first, second)

        assertEquals("from-first", result)
    }

    @Test
    fun theEarlierSendWinsWhenBothChannelsEventuallySend() = runTest {
        val first = Channel<Int>()
        val second = Channel<Int>()
        launch {
            delay(100)
            first.send(1)
        }
        launch {
            delay(500)
            second.send(2)
        }

        val result = firstAvailable(first, second)

        assertEquals(1, result)
    }
}
