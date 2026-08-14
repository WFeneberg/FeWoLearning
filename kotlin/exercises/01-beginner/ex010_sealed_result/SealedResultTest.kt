package fewolearning.exercises.beginner.ex010_sealed_result

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class SealedResultTest {

    @Test
    fun describeSuccessIncludesTheValue() {
        assertEquals("Success: 42", describe(ParseResult.Success(42)))
    }

    @Test
    fun describeFailureIncludesTheReason() {
        assertEquals("Failure: not a number", describe(ParseResult.Failure("not a number")))
    }
}
