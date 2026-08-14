package fewolearning.exercises.beginner.ex030_string_builder;

import java.util.List;

/*
Exercise 030 - StringBuilder (reference solution).
*/
public final class StringBuilderReport {
    private StringBuilderReport() {
    }

    public static String buildCsvLine(List<String> fields) {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < fields.size(); i++) {
            if (i > 0) {
                builder.append(',');
            }
            builder.append(fields.get(i));
        }
        return builder.toString();
    }

    public static String reverse(String value) {
        return new StringBuilder(value).reverse().toString();
    }
}
