package fewolearning.exercises.advanced.ex082_sealed_error_hierarchy

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertNotEquals

class DomainErrorTest {

    @Test
    fun eachErrorVariantProducesADistinctUserFacingMessage() {
        val notFound = userMessage(DomainError.NotFound("42"))
        val validation = userMessage(DomainError.Validation("email", "must contain @"))
        val unauthorized = userMessage(DomainError.Unauthorized)

        assertEquals("Could not find item with id 42.", notFound)
        assertEquals("Invalid email: must contain @.", validation)
        assertEquals("You are not authorized to perform this action.", unauthorized)
        assertNotEquals(notFound, validation)
        assertNotEquals(validation, unauthorized)
    }
}
