package fewolearning.exercises.beginner.ex006_elvis_operator

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ElvisOperatorTest {

    @Test
    fun resolveNameReturnsTheNameWhenItIsNonBlank() {
        assertEquals("Alice", resolveName("Alice"))
    }

    @Test
    fun resolveNameFallsBackToDefaultWhenNull() {
        assertEquals("Anonymous", resolveName(null))
    }

    @Test
    fun resolveNameFallsBackToDefaultWhenBlank() {
        assertEquals("Anonymous", resolveName(""))
        assertEquals("Anonymous", resolveName("   "))
    }
}
