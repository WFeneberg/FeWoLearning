package fewolearning.exercises.intermediate.ex041_inline_higher_order

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertNull

class InlineHigherOrderTest {

    @Test
    fun returnsTheBlocksResultWhenItSucceeds() {
        assertEquals(42, runOrNull { 42 })
    }

    @Test
    fun returnsNullWhenTheBlockThrows() {
        assertNull(runOrNull<Int> { throw RuntimeException("boom") })
    }

    @Test
    fun beingInlineAllowsANonLocalReturnFromInsideTheLambda() {
        fun helper(): Int {
            runOrNull {
                return 7
            }
            return -1
        }

        assertEquals(7, helper())
    }
}
