package fewolearning.exercises.beginner.ex015_map_count_words

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertTrue

class MapCountWordsTest {

    @Test
    fun countWordsTalliesOccurrencesOfEachWord() {
        val counts = countWords(listOf("a", "b", "a", "c", "b", "a"))

        assertEquals(3, counts["a"])
        assertEquals(2, counts["b"])
        assertEquals(1, counts["c"])
    }

    @Test
    fun countWordsReturnsAnEmptyMapForAnEmptyList() {
        assertTrue(countWords(emptyList()).isEmpty())
    }
}
