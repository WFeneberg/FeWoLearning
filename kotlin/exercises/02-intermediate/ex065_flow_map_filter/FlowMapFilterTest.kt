package fewolearning.exercises.intermediate.ex065_flow_map_filter

import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.flow.toList
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class FlowMapFilterTest {

    @Test
    fun keepsOnlyEvenNumbersThenSquaresThem() = runTest {
        val source = flowOf(1, 2, 3, 4, 5, 6)

        val result = evenSquaresFlow(source).toList()

        assertEquals(listOf(4, 16, 36), result)
    }

    @Test
    fun producesAnEmptyFlowWhenThereAreNoEvenNumbers() = runTest {
        val source = flowOf(1, 3, 5)

        val result = evenSquaresFlow(source).toList()

        assertEquals(emptyList<Int>(), result)
    }
}
