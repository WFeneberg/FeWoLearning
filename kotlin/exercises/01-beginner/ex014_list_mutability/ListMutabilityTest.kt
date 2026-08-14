package fewolearning.exercises.beginner.ex014_list_mutability

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertIterableEquals

class ListMutabilityTest {

    @Test
    fun withAppendedReturnsANewListWithTheItemAtTheEnd() {
        val result = withAppended(listOf("a", "b"), "c")

        assertIterableEquals(listOf("a", "b", "c"), result)
    }

    @Test
    fun withAppendedDoesNotMutateTheOriginalList() {
        val original = listOf("a", "b")

        withAppended(original, "c")

        assertEquals(2, original.size)
        assertIterableEquals(listOf("a", "b"), original)
    }
}
