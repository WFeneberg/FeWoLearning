package fewolearning.exercises.beginner.ex014_text_block_report;

/*
Exercise 014 - Text block report (reference solution).
*/
public final class ReceiptReport {
    private ReceiptReport() {
    }

    public static String render(String customerName, int itemCount, double total) {
        String template = """
                Receipt for %s
                Items: %d
                Total: $%.2f
                """;
        return template.formatted(customerName, itemCount, total);
    }
}
