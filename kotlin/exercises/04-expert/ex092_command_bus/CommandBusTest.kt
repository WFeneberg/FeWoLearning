package fewolearning.exercises.expert.ex092_command_bus

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertThrows

data class CreateUser(val name: String) : Command
data class DeleteUser(val id: Int) : Command

class CommandBusTest {

    @Test
    fun dispatchesEachCommandToTheHandlerRegisteredForItsRuntimeType() {
        val bus = CommandBus()
        val created = mutableListOf<String>()
        val deleted = mutableListOf<Int>()
        bus.register(CreateUser::class.java) { created.add(it.name) }
        bus.register(DeleteUser::class.java) { deleted.add(it.id) }

        bus.dispatch(CreateUser("Ada"))
        bus.dispatch(DeleteUser(7))
        bus.dispatch(CreateUser("Grace"))

        assertEquals(listOf("Ada", "Grace"), created)
        assertEquals(listOf(7), deleted)
    }

    @Test
    fun dispatchingAnUnregisteredCommandTypeThrows() {
        val bus = CommandBus()
        bus.register(DeleteUser::class.java) { }

        assertThrows(NoSuchElementException::class.java) { bus.dispatch(CreateUser("Ada")) }
    }
}
