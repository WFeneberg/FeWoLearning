package fewolearning.exercises.beginner.ex016_higher_order_basics

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertIterableEquals

class HigherOrderBasicsTest {

    @Test
    fun transformAllDoublesEachNumber() {
        val result = transformAll(listOf(1, 2, 3)) { it * 2 }

        assertIterableEquals(listOf(2, 4, 6), result)
    }

    @Test
    fun transformAllCanChangeTheElementType() {
        val result = transformAll(listOf("a", "bb", "ccc")) { it.length }

        assertIterableEquals(listOf(1, 2, 3), result)
    }

    @Test
    fun transformAllReturnsAnEmptyListForAnEmptyInput() {
        val result = transformAll(emptyList<Int>()) { it.toString() }

        assertIterableEquals(emptyList<String>(), result)
    }
}
