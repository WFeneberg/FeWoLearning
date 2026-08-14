package fewolearning.exercises.beginner.ex029_result_run_catching

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ResultRunCatchingTest {

    @Test
    fun parseOrDefaultReturnsTheParsedIntegerWhenValid() {
        assertEquals(42, parseOrDefault("42", 0))
    }

    @Test
    fun parseOrDefaultReturnsTheDefaultWhenParsingFails() {
        assertEquals(-1, parseOrDefault("abc", -1))
    }

    @Test
    fun parseOrDefaultReturnsTheDefaultForABlankString() {
        assertEquals(7, parseOrDefault("", 7))
    }
}
