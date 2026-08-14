package fewolearning.exercises.intermediate.ex043_nullable_receiver_ext

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class NullableReceiverExtTest {

    @Test
    fun returnsTheDefaultWhenTheReceiverIsNull() {
        val receiver: String? = null

        assertEquals("fallback", receiver.orDefault("fallback"))
    }

    @Test
    fun returnsTheDefaultWhenTheReceiverIsBlank() {
        assertEquals("fallback", "   ".orDefault("fallback"))
    }

    @Test
    fun returnsTheReceiverWhenItHasContent() {
        assertEquals("hello", "hello".orDefault("fallback"))
    }
}
