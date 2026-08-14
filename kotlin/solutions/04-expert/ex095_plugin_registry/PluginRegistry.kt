package fewolearning.exercises.expert.ex095_plugin_registry

/** runByName throws NoSuchElementException when asked for a name that was never registered. */
interface Plugin {
    val name: String
    fun execute(): String
}

class PluginRegistry {
    private val plugins = mutableMapOf<String, Plugin>()

    fun register(plugin: Plugin) {
        plugins[plugin.name] = plugin
    }

    fun runByName(name: String): String =
        (plugins[name] ?: throw NoSuchElementException("No plugin registered as '$name'")).execute()
}
