package fewolearning.exercises.beginner.ex014_text_block_report;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ReceiptReportTest {

    @Test
    void renderIncludesTheCustomerNameItemCountAndTotal() {
        String receipt = ReceiptReport.render("Ann", 3, 42.5);

        assertTrue(receipt.contains("Ann"));
        assertTrue(receipt.contains("3"));
        assertTrue(receipt.contains("42.50"));
    }

    @Test
    void renderProducesTheExactExpectedLayout() {
        String expected = """
                Receipt for Ann
                Items: 3
                Total: $42.50
                """;

        assertEquals(expected, ReceiptReport.render("Ann", 3, 42.5));
    }
}
