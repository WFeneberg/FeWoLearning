package fewolearning.exercises.beginner.ex025_generic_box

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertNull

class BoxTest {

    @Test
    fun getReturnsNullForAFreshlyConstructedEmptyBox() {
        val box = Box<String>()

        assertNull(box.get())
    }

    @Test
    fun setThenGetReturnsTheStoredValue() {
        val box = Box<String>()

        box.set("hello")

        assertEquals("hello", box.get())
    }

    @Test
    fun getReturnsTheInitialConstructorValueWhenProvided() {
        val box = Box("start")

        assertEquals("start", box.get())
    }

    @Test
    fun setOverwritesAPreviouslyStoredValue() {
        val box = Box(1)

        box.set(2)

        assertEquals(2, box.get())
    }
}
