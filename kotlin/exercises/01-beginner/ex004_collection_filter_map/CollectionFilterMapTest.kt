package fewolearning.exercises.beginner.ex004_collection_filter_map

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class CollectionFilterMapTest {

    @Test
    fun evenSquaresFiltersThenSquares() {
        assertEquals(listOf(4, 16), evenSquares(listOf(1, 2, 3, 4, 5)))
    }

    @Test
    fun evenSquaresOfAnEmptyListIsEmpty() {
        assertEquals(emptyList<Int>(), evenSquares(emptyList()))
    }
}
