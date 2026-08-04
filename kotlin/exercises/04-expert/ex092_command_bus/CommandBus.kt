package fewolearning.exercises.expert.ex092_command_bus

/*
Exercise 092 - Command bus (expert).

Goal:   Route commands by their runtime type to registered handlers.
Drills: typed command dispatch.
*/
interface Command

class CommandBus {
    private val handlers = mutableMapOf<Class<out Command>, (Command) -> Unit>()

    fun <T : Command> register(type: Class<T>, handler: (T) -> Unit) {
        TODO()
    }

    fun dispatch(command: Command) {
        TODO()
    }
}
