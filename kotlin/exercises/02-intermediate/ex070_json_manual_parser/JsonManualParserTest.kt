package fewolearning.exercises.intermediate.ex070_json_manual_parser

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertThrows

class JsonManualParserTest {

    @Test
    fun parsesAFlatObjectWithMultipleStringEntries() {
        val result = parseFlatJsonObject("{\"name\": \"Ada\", \"role\": \"engineer\"}")

        assertEquals(mapOf("name" to "Ada", "role" to "engineer"), result)
    }

    @Test
    fun parsesAnEmptyObjectAsAnEmptyMap() {
        assertEquals(emptyMap<String, String>(), parseFlatJsonObject("{}"))
    }

    @Test
    fun throwsOnMalformedInput() {
        assertThrows(IllegalArgumentException::class.java) {
            parseFlatJsonObject("{\"name\": \"Ada\"")
        }
    }

    @Test
    fun throwsWhenTheInputIsNotAnObject() {
        assertThrows(IllegalArgumentException::class.java) {
            parseFlatJsonObject("[\"name\", \"Ada\"]")
        }
    }
}
