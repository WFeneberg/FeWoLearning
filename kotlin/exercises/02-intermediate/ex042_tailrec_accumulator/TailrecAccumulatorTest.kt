package fewolearning.exercises.intermediate.ex042_tailrec_accumulator

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class TailrecAccumulatorTest {

    @Test
    fun sumsAllNumbersInTheList() {
        assertEquals(10, sumTailrec(listOf(1, 2, 3, 4)))
    }

    @Test
    fun returnsZeroForAnEmptyList() {
        assertEquals(0, sumTailrec(emptyList()))
    }
}
