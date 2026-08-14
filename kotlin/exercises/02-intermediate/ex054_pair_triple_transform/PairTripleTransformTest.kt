package fewolearning.exercises.intermediate.ex054_pair_triple_transform

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class PairTripleTransformTest {

    @Test
    fun computesMinMaxAndAverageAsATriple() {
        val result = minMaxAverage(listOf(1, 2, 3, 4, 5))

        assertEquals(Triple(1, 5, 3.0), result)
    }

    @Test
    fun handlesASingleElementList() {
        assertEquals(Triple(7, 7, 7.0), minMaxAverage(listOf(7)))
    }
}
