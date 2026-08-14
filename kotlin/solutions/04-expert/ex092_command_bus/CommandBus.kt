package fewolearning.exercises.expert.ex092_command_bus

/**
 * Handlers are stored keyed by the exact `Class` they were registered under, and dispatch
 * looks the incoming command's runtime `Class` up in that map (`command::class.java`,
 * converting Kotlin's `KClass` to the `java.lang.Class` the map already uses as its key).
 */
interface Command

class CommandBus {
    private val handlers = mutableMapOf<Class<out Command>, (Command) -> Unit>()

    fun <T : Command> register(type: Class<T>, handler: (T) -> Unit) {
        @Suppress("UNCHECKED_CAST")
        handlers[type] = handler as (Command) -> Unit
    }

    fun dispatch(command: Command) {
        val handler = handlers[command::class.java]
            ?: throw NoSuchElementException("No handler registered for ${command::class.java}")
        handler(command)
    }
}
