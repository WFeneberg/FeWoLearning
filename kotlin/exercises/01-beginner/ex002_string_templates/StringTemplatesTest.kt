package fewolearning.exercises.beginner.ex002_string_templates

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class StringTemplatesTest {

    @Test
    fun greetIncludesNameAndAge() {
        assertEquals("Hello, John! You are 30 years old.", greet("John", 30))
    }

    @Test
    fun orderSummaryBuildsAMultilineReport() {
        val expected = "Item: Widget\nQuantity: 3\nUnit price: 2.5\nTotal: 7.5"

        assertEquals(expected, orderSummary("Widget", 3, 2.5))
    }
}
