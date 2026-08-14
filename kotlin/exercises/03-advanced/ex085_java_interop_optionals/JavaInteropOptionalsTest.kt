package fewolearning.exercises.advanced.ex085_java_interop_optionals

import java.util.Optional
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertFalse
import org.junit.jupiter.api.Assertions.assertNull
import org.junit.jupiter.api.Assertions.assertTrue

class JavaInteropOptionalsTest {

    @Test
    fun toNullableUnwrapsAPresentOptional() {
        assertEquals("value", toNullable(Optional.of("value")))
    }

    @Test
    fun toNullableReturnsNullForAnEmptyOptional() {
        assertNull(toNullable(Optional.empty()))
    }

    @Test
    fun toOptionalWrapsANonNullValueAsPresent() {
        val optional = toOptional("value")

        assertTrue(optional.isPresent)
        assertEquals("value", optional.get())
    }

    @Test
    fun toOptionalWrapsNullAsEmpty() {
        val optional = toOptional(null)

        assertFalse(optional.isPresent)
    }
}
