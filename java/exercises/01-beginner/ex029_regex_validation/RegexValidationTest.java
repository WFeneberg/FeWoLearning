package fewolearning.exercises.beginner.ex029_regex_validation;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class RegexValidationTest {

    @Test
    void isValidEmailAcceptsAStandardShapedAddress() {
        assertTrue(RegexValidation.isValidEmail("ann.smith@example.com"));
    }

    @Test
    void isValidEmailRejectsAnAddressMissingAtSign() {
        assertFalse(RegexValidation.isValidEmail("ann.smith-example.com"));
    }

    @Test
    void isValidEmailRejectsAnAddressMissingADomainSuffix() {
        assertFalse(RegexValidation.isValidEmail("ann@example"));
    }

    @Test
    void isValidEmailRejectsBlank() {
        assertFalse(RegexValidation.isValidEmail(""));
    }

    @Test
    void extractDomainReturnsThePartAfterTheAtSign() {
        assertEquals("example.com", RegexValidation.extractDomain("ann.smith@example.com"));
    }

    @Test
    void extractDomainThrowsForAnInvalidEmail() {
        assertThrows(IllegalArgumentException.class, () -> RegexValidation.extractDomain("not-an-email"));
    }
}
