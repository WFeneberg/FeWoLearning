package fewolearning.exercises.beginner.ex003_string_formatting;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class StringFormattingTest {

    @Test
    void initialsUppercasesTheFirstLetterOfEachName() {
        assertEquals("J.D.", StringFormatting.initials("John", "Doe"));
    }

    @Test
    void initialsUppercasesEvenWhenInputIsLowercase() {
        assertEquals("A.L.", StringFormatting.initials("ada", "lovelace"));
    }

    @Test
    void formatReceiptLineIncludesQuantityPriceAndTotal() {
        assertEquals("3x Widget @ $2.50 = $7.50", StringFormatting.formatReceiptLine("Widget", 3, 2.50));
    }

    @Test
    void formatReceiptLineRoundsPricesToTwoDecimals() {
        assertEquals("1x Gadget @ $9.99 = $9.99", StringFormatting.formatReceiptLine("Gadget", 1, 9.99));
    }
}
