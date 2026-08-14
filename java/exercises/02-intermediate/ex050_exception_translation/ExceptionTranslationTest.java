package fewolearning.exercises.intermediate.ex050_exception_translation;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertInstanceOf;
import static org.junit.jupiter.api.Assertions.assertThrows;

class ExceptionTranslationTest {

    @Test
    void parseConfigValueParsesAValidNumber() {
        assertEquals(42, ExceptionTranslation.parseConfigValue("42"));
    }

    @Test
    void parseConfigValueWrapsANumberFormatExceptionInAConfigParseException() {
        ExceptionTranslation.ConfigParseException thrown = assertThrows(
                ExceptionTranslation.ConfigParseException.class,
                () -> ExceptionTranslation.parseConfigValue("not-a-number"));

        assertInstanceOf(NumberFormatException.class, thrown.getCause());
    }
}
