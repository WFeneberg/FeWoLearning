package fewolearning.exercises.intermediate.ex044_infix_functions

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class InfixFunctionsTest {

    @Test
    fun upToBuildsARangeFromTheReceiverToTheArgument() {
        assertEquals(Range(1, 5), 1 upTo 5)
    }

    @Test
    fun upToWorksWithoutInfixNotationToo() {
        assertEquals(Range(2, 2), 2.upTo(2))
    }
}
