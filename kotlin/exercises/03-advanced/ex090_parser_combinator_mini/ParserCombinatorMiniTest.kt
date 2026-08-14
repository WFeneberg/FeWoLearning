package fewolearning.exercises.advanced.ex090_parser_combinator_mini

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertNull

class ParserCombinatorMiniTest {

    @Test
    fun charParserMatchesTheExpectedLeadingCharacter() {
        val result = charParser('a')("abc")

        assertEquals('a' to "bc", result)
    }

    @Test
    fun charParserFailsOnAMismatchedLeadingCharacter() {
        val result = charParser('a')("xyz")

        assertNull(result)
    }

    @Test
    fun repeatParserCollectsEveryConsecutiveMatchAndStopsAtTheFirstMismatch() {
        val result = repeatParser(charParser('a'))("aaab")

        assertEquals(listOf('a', 'a', 'a') to "b", result)
    }

    @Test
    fun repeatParserSucceedsWithAnEmptyListWhenThereIsNoMatchAtAll() {
        val result = repeatParser(charParser('a'))("bbb")

        assertEquals(emptyList<Char>() to "bbb", result)
    }
}
