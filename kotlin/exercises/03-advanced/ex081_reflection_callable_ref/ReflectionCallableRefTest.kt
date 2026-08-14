package fewolearning.exercises.advanced.ex081_reflection_callable_ref

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ReflectionCallableRefTest {

    @Test
    fun appliesAStringLengthCallableReferenceToEveryElement() {
        val words = listOf("kotlin", "jvm", "coroutines")

        val lengths = applyAll(words, String::length)

        assertEquals(listOf(6, 3, 10), lengths)
    }
}
