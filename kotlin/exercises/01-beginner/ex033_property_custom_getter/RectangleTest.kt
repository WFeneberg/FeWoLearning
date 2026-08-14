package fewolearning.exercises.beginner.ex033_property_custom_getter

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class RectangleTest {

    @Test
    fun areaMultipliesWidthAndHeight() {
        assertEquals(12, Rectangle(3, 4).area)
    }

    @Test
    fun perimeterSumsAllFourSides() {
        assertEquals(14, Rectangle(3, 4).perimeter)
    }

    @Test
    fun areaAndPerimeterAreRecomputedForDifferentInstances() {
        assertEquals(25, Rectangle(5, 5).area)
        assertEquals(20, Rectangle(5, 5).perimeter)
    }
}
