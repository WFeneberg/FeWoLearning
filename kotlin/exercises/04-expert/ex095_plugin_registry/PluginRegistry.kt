package fewolearning.exercises.expert.ex095_plugin_registry

/*
Exercise 095 - Plugin registry (expert).

Goal:   Register named plugins and look them up by name at runtime.
Drills: extension points, discovery.
*/
interface Plugin {
    val name: String
    fun execute(): String
}

class PluginRegistry {
    private val plugins = mutableMapOf<String, Plugin>()

    fun register(plugin: Plugin) {
        TODO()
    }

    fun runByName(name: String): String {
        TODO()
    }
}
