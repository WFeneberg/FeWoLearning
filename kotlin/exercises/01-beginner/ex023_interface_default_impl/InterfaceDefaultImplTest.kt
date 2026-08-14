package fewolearning.exercises.beginner.ex023_interface_default_impl

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class InterfaceDefaultImplTest {

    @Test
    fun greetUsesTheDefaultImplementationFromTheInterface() {
        val greeter = FormalGreeter("Alice")

        assertEquals("Hello, my name is Alice.", greeter.greet())
    }
}
