package fewolearning.exercises.advanced.ex087_csv_parser;

import java.util.List;

/*
Exercise 087 - CSV parser (advanced).

Goal:   Parse CSV lines into rows, collecting malformed lines as recoverable errors.
Drills: parsing, validation, recoverable errors.
*/
public final class CsvParser {
    private CsvParser() {
    }

    public record ParseResult(List<List<String>> rows, List<String> errors) {
    }

    public static ParseResult parse(List<String> lines, int expectedColumnCount) {
        throw new UnsupportedOperationException("TODO");
    }
}
