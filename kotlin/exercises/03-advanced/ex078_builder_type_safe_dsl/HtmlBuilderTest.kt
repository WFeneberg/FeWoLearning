package fewolearning.exercises.advanced.ex078_builder_type_safe_dsl

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class HtmlBuilderTest {

    @Test
    fun buildsMultipleParagraphsInDeclarationOrder() {
        val document = html {
            paragraph("first")
            paragraph("second")
        }

        assertEquals("<p>first</p>\n<p>second</p>", document)
    }
}
