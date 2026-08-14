package fewolearning.exercises.beginner.ex005_nullable_basics

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class NullableBasicsTest {

    @Test
    fun describeLengthReportsLengthWhenPresent() {
        assertEquals("length: 5", describeLength("hello"))
    }

    @Test
    fun describeLengthHandlesNull() {
        assertEquals("no value", describeLength(null))
    }
}
