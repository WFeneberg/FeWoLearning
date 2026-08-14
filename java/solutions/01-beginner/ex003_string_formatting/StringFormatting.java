package fewolearning.exercises.beginner.ex003_string_formatting;

/*
Exercise 003 - String formatting (reference solution).
*/
public final class StringFormatting {
    private StringFormatting() {
    }

    public static String initials(String firstName, String lastName) {
        return Character.toUpperCase(firstName.charAt(0)) + "." + Character.toUpperCase(lastName.charAt(0)) + ".";
    }

    public static String formatReceiptLine(String itemName, int quantity, double unitPrice) {
        return String.format("%dx %s @ $%.2f = $%.2f", quantity, itemName, unitPrice, quantity * unitPrice);
    }
}
