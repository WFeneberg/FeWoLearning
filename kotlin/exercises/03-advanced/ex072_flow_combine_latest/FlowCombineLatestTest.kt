package fewolearning.exercises.advanced.ex072_flow_combine_latest

import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.flow.toList
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class FlowCombineLatestTest {

    @Test
    fun pairsEachEmissionWithTheOtherFlowsLatestValue() = runTest {
        // Real delay()s between the "numbers" emissions give combine's internal
        // collector a genuine suspension point to propagate each combined value
        // downstream before the next upstream emission arrives - without them
        // (e.g. plain flowOf(1, 2, 3) with no gaps), rapid-fire emissions could
        // be conflated and only the final combination would survive.
        val numbers = flow {
            emit(1)
            delay(10)
            emit(2)
            delay(10)
            emit(3)
        }
        val words = flowOf("a")

        val pairs = combinePairs(numbers, words).toList()

        assertEquals(listOf(1 to "a", 2 to "a", 3 to "a"), pairs)
    }
}
