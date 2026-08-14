package fewolearning.exercises.beginner.ex030_string_builder;

import org.junit.jupiter.api.Test;

import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;

class StringBuilderReportTest {

    @Test
    void buildCsvLineJoinsFieldsWithCommas() {
        assertEquals("id,name,total", StringBuilderReport.buildCsvLine(List.of("id", "name", "total")));
    }

    @Test
    void buildCsvLineOfASingleFieldHasNoComma() {
        assertEquals("solo", StringBuilderReport.buildCsvLine(List.of("solo")));
    }

    @Test
    void buildCsvLineOfNoFieldsIsEmpty() {
        assertEquals("", StringBuilderReport.buildCsvLine(List.of()));
    }

    @Test
    void reverseReversesTheCharacters() {
        assertEquals("cba", StringBuilderReport.reverse("abc"));
    }

    @Test
    void reverseOfAnEmptyStringIsEmpty() {
        assertEquals("", StringBuilderReport.reverse(""));
    }
}
