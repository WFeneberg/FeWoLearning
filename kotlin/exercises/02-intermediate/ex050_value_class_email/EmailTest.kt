package fewolearning.exercises.intermediate.ex050_value_class_email

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertThrows

class EmailTest {

    @Test
    fun constructsSuccessfullyFromAValidLookingAddress() {
        val email = Email("ada@example.com")

        assertEquals("ada@example.com", email.raw)
    }

    @Test
    fun rejectsAValueWithoutAnAtSign() {
        assertThrows(IllegalArgumentException::class.java) { Email("not-an-email") }
    }

    @Test
    fun rejectsAValueWithAnEmptyLocalOrDomainPart() {
        assertThrows(IllegalArgumentException::class.java) { Email("@example.com") }
        assertThrows(IllegalArgumentException::class.java) { Email("ada@") }
    }
}
