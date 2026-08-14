package fewolearning.exercises.expert.ex093_schema_form_renderer

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class SchemaFormRendererTest {

    @Test
    fun renderLabelsMarksRequiredFieldsWithATrailingAsterisk() {
        val schema = listOf(
            FieldSchema("email", required = true),
            FieldSchema("nickname", required = false)
        )

        assertEquals(listOf("email *", "nickname"), renderLabels(schema))
    }

    @Test
    fun validateReportsRequiredFieldsThatAreMissingOrBlank() {
        val schema = listOf(
            FieldSchema("email", required = true),
            FieldSchema("phone", required = true),
            FieldSchema("address", required = true),
            FieldSchema("nickname", required = false)
        )
        // phone is present but blank, address is entirely absent from the submitted values.
        val values = mapOf("email" to "a@b.com", "phone" to "   ")

        assertEquals(listOf("phone", "address"), validate(schema, values))
    }

    @Test
    fun validateReturnsEmptyWhenAllRequiredFieldsArePresentAndNonBlank() {
        val schema = listOf(
            FieldSchema("email", required = true),
            FieldSchema("nickname", required = false)
        )
        val values = mapOf("email" to "a@b.com")

        assertEquals(emptyList<String>(), validate(schema, values))
    }
}
