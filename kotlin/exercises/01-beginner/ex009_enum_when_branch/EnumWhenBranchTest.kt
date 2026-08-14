package fewolearning.exercises.beginner.ex009_enum_when_branch

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class EnumWhenBranchTest {

    @Test
    fun actionForRedIsStop() {
        assertEquals("Stop", actionFor(TrafficLight.RED))
    }

    @Test
    fun actionForYellowIsCaution() {
        assertEquals("Caution", actionFor(TrafficLight.YELLOW))
    }

    @Test
    fun actionForGreenIsGo() {
        assertEquals("Go", actionFor(TrafficLight.GREEN))
    }
}
