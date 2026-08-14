package fewolearning.exercises.intermediate.ex065_locale_number_format;

import java.text.NumberFormat;
import java.util.Locale;

/*
Exercise 065 - Locale number format (reference solution).
*/
public final class LocaleNumberFormat {
    private LocaleNumberFormat() {
    }

    public static String formatCurrency(double amount, Locale locale) {
        return NumberFormat.getCurrencyInstance(locale).format(amount);
    }
}
