package fewolearning.exercises.advanced.ex087_csv_parser;

import java.util.List;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

class CsvParserTest {

    @Test
    void parsesWellFormedLinesIntoRows() {
        CsvParser.ParseResult result = CsvParser.parse(List.of("a,b,c", "d,e,f"), 3);

        assertEquals(List.of(List.of("a", "b", "c"), List.of("d", "e", "f")), result.rows());
        assertTrue(result.errors().isEmpty());
    }

    @Test
    void collectsMalformedLinesAsRecoverableErrorsWithoutFailingTheWholeParse() {
        CsvParser.ParseResult result = CsvParser.parse(List.of("a,b,c", "only-one-column", "d,e,f"), 3);

        assertEquals(List.of(List.of("a", "b", "c"), List.of("d", "e", "f")), result.rows());
        assertEquals(1, result.errors().size());
        assertTrue(result.errors().get(0).contains("only-one-column"));
    }

    @Test
    void anEmptyInputProducesNoRowsAndNoErrors() {
        CsvParser.ParseResult result = CsvParser.parse(List.of(), 3);

        assertTrue(result.rows().isEmpty());
        assertTrue(result.errors().isEmpty());
    }

    @Test
    void aTrailingEmptyColumnIsPreservedRatherThanDropped() {
        CsvParser.ParseResult result = CsvParser.parse(List.of("a,b,"), 3);

        assertEquals(List.of(List.of("a", "b", "")), result.rows());
        assertTrue(result.errors().isEmpty());
    }
}
