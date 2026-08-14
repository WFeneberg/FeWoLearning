package fewolearning.exercises.advanced.ex087_csv_parser;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

/*
Exercise 087 - CSV parser (reference solution).
*/
public final class CsvParser {
    private CsvParser() {
    }

    public record ParseResult(List<List<String>> rows, List<String> errors) {
    }

    public static ParseResult parse(List<String> lines, int expectedColumnCount) {
        List<List<String>> rows = new ArrayList<>();
        List<String> errors = new ArrayList<>();
        for (int i = 0; i < lines.size(); i++) {
            String line = lines.get(i);
            List<String> columns = Arrays.asList(line.split(",", -1));
            if (columns.size() != expectedColumnCount) {
                errors.add("Line " + (i + 1) + ": expected " + expectedColumnCount
                        + " columns but found " + columns.size() + ": " + line);
            } else {
                rows.add(new ArrayList<>(columns));
            }
        }
        return new ParseResult(rows, errors);
    }
}
