package fewolearning.exercises.intermediate.ex056_exception_wrapping

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertThrows
import org.junit.jupiter.api.Assertions.assertTrue

class ExceptionWrappingTest {

    @Test
    fun parsesAValidNumericString() {
        assertEquals(42, parseConfigValue("42"))
    }

    @Test
    fun wrapsANumberFormatExceptionInADomainExceptionWithTheOriginalAsCause() {
        val thrown = assertThrows(ConfigParseException::class.java) { parseConfigValue("abc") }

        assertTrue(thrown.cause is NumberFormatException)
    }
}
