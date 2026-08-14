package fewolearning.exercises.advanced.ex087_object_expression_listener

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ClickListenerTest {

    @Test
    fun onClickAppendsALogEntryWithTheCoordinatesForEachCall() {
        val log = mutableListOf<String>()
        val listener = loggingListener(log)

        listener.onClick(10, 20)
        listener.onClick(3, 4)

        assertEquals(listOf("clicked at (10, 20)", "clicked at (3, 4)"), log)
    }
}
