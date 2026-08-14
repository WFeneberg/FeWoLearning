package fewolearning.exercises.advanced.ex083_dsl_marker_scope

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class FormBuilderTest {

    @Test
    fun collectsEachDeclaredFieldInDeclarationOrder() {
        val fields = form {
            field("name")
            field("email")
        }

        assertEquals(listOf("name", "email"), fields)
    }
}
