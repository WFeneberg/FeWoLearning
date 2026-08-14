package fewolearning.exercises.advanced.ex072_flow_combine_latest

import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.flow.toList
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class FlowCombineLatestTest {

    @Test
    fun pairsEachEmissionWithTheOtherFlowsLatestValue() = runTest {
        val numbers = flowOf(1, 2, 3)
        val words = flowOf("a")

        val pairs = combinePairs(numbers, words).toList()

        assertEquals(listOf(1 to "a", 2 to "a", 3 to "a"), pairs)
    }
}
