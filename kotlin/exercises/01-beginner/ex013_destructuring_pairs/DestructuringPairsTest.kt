package fewolearning.exercises.beginner.ex013_destructuring_pairs

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class DestructuringPairsTest {

    @Test
    fun splitFullNameDestructuresIntoFirstAndLast() {
        val (first, last) = splitFullName("John Doe")

        assertEquals("John", first)
        assertEquals("Doe", last)
    }

    @Test
    fun splitFullNameKeepsAMultiWordLastNameTogether() {
        val (first, last) = splitFullName("Mary Jane Watson")

        assertEquals("Mary", first)
        assertEquals("Jane Watson", last)
    }

    @Test
    fun splitFullNameReturnsAnEmptyLastNameForASingleWord() {
        val (first, last) = splitFullName("Cher")

        assertEquals("Cher", first)
        assertEquals("", last)
    }
}
