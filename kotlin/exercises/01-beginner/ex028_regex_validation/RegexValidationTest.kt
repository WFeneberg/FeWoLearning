package fewolearning.exercises.beginner.ex028_regex_validation

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertTrue
import org.junit.jupiter.api.Assertions.assertFalse
import org.junit.jupiter.api.Assertions.assertNull
import org.junit.jupiter.api.Assertions.assertNotNull

class RegexValidationTest {

    @Test
    fun isValidEmailIsTrueForAWellFormedAddress() {
        assertTrue(isValidEmail("john@example.com"))
    }

    @Test
    fun isValidEmailIsFalseForAMalformedAddress() {
        assertFalse(isValidEmail("not-an-email"))
        assertFalse(isValidEmail("missing@domain"))
    }

    @Test
    fun userAndDomainDestructuresAValidAddress() {
        val result = userAndDomain("john@example.com")

        assertNotNull(result)
        val (user, domain) = result!!
        assertEquals("john", user)
        assertEquals("example.com", domain)
    }

    @Test
    fun userAndDomainReturnsNullForAnInvalidAddress() {
        assertNull(userAndDomain("not-an-email"))
    }
}
