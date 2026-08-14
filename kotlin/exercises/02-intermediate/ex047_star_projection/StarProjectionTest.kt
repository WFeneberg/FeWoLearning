package fewolearning.exercises.intermediate.ex047_star_projection

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class StarProjectionTest {

    @Test
    fun readsTheSizeOfAListOfIntegers() {
        assertEquals(3, sizeOfAnyList(listOf(1, 2, 3)))
    }

    @Test
    fun readsTheSizeOfAListOfStringsRegardlessOfElementType() {
        assertEquals(2, sizeOfAnyList(listOf("a", "b")))
    }

    @Test
    fun readsTheSizeOfAnEmptyList() {
        assertEquals(0, sizeOfAnyList(emptyList<Any?>()))
    }
}
