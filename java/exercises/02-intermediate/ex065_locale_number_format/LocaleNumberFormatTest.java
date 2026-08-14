package fewolearning.exercises.intermediate.ex065_locale_number_format;

import java.util.Locale;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class LocaleNumberFormatTest {

    @Test
    void formatsCurrencyForUnitedStatesLocale() {
        String formatted = LocaleNumberFormat.formatCurrency(1234.5, Locale.US);

        assertEquals("$1,234.50", formatted);
    }

    @Test
    void formatsCurrencyForGermanLocale() {
        String formatted = LocaleNumberFormat.formatCurrency(1234.5, Locale.GERMANY);

        // Note: the JDK's CLDR-based currency format for de_DE separates the
        // amount from the currency symbol with a NO-BREAK SPACE (U+00A0), not
        // an ordinary space - the literal below embeds that exact character.
        assertEquals("1.234,50 €", formatted);
    }
}
