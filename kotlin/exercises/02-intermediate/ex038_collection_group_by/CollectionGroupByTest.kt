package fewolearning.exercises.intermediate.ex038_collection_group_by

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class CollectionGroupByTest {

    @Test
    fun countsHowManyWordsFallIntoEachLengthGroup() {
        val words = listOf("a", "bb", "cc", "ddd", "e")

        val result = countByLength(words)

        assertEquals(mapOf(1 to 2, 2 to 2, 3 to 1), result)
    }

    @Test
    fun returnsAnEmptyMapForAnEmptyList() {
        assertEquals(emptyMap<Int, Int>(), countByLength(emptyList()))
    }
}
