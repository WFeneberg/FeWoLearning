package fewolearning.exercises.beginner.ex001_val_var_basics

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ValVarBasicsTest {

    @Test
    fun areaMultipliesWidthAndHeight() {
        assertEquals(20, area(4, 5))
    }

    @Test
    fun perimeterSumsAllFourSides() {
        assertEquals(18, perimeter(4, 5))
    }
}
