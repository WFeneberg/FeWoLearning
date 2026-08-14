package fewolearning.exercises.beginner.ex018_scope_let_run

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ScopeLetRunTest {

    @Test
    fun describeOrDefaultDescribesANonNullValue() {
        assertEquals("Value: hi", describeOrDefault("hi", "N/A"))
    }

    @Test
    fun describeOrDefaultFallsBackForNull() {
        assertEquals("N/A", describeOrDefault(null, "N/A"))
    }

    @Test
    fun computeAreaMultipliesWidthAndHeight() {
        assertEquals(12, computeArea(3, 4))
    }
}
