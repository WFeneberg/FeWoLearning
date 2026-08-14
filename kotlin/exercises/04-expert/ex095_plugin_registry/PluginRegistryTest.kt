package fewolearning.exercises.expert.ex095_plugin_registry

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertThrows

class PluginRegistryTest {

    private class UppercasePlugin(private val input: String) : Plugin {
        override val name = "uppercase"
        override fun execute(): String = input.uppercase()
    }

    private class GreetingPlugin : Plugin {
        override val name = "greeting"
        override fun execute(): String = "hello"
    }

    @Test
    fun runByNameExecutesThePluginRegisteredUnderThatName() {
        val registry = PluginRegistry()
        registry.register(UppercasePlugin("hi"))
        registry.register(GreetingPlugin())

        assertEquals("HI", registry.runByName("uppercase"))
        assertEquals("hello", registry.runByName("greeting"))
    }

    @Test
    fun runByNameForAnUnregisteredNameThrows() {
        val registry = PluginRegistry()
        registry.register(GreetingPlugin())

        assertThrows(NoSuchElementException::class.java) { registry.runByName("missing") }
    }
}
