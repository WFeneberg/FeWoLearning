package fewolearning.exercises.intermediate.ex064_channel_pipeline

import kotlinx.coroutines.channels.Channel
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ChannelPipelineTest {

    @Test
    fun pipeThroughDeliversAllValuesInOrderOverAnUnbufferedChannel() = runTest {
        val values = listOf(1, 2, 3, 4, 5)

        val result = pipeThrough(values, Channel())

        assertEquals(values, result)
    }

    @Test
    fun pipeThroughHandlesAnEmptyList() = runTest {
        val result = pipeThrough(emptyList(), Channel())

        assertEquals(emptyList<Int>(), result)
    }
}
