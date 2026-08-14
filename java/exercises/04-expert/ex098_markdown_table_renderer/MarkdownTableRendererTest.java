package fewolearning.exercises.expert.ex098_markdown_table_renderer;

import java.util.List;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class MarkdownTableRendererTest {

    @Test
    void rendersAHeaderSeparatorAndDataRowsWithColumnsPaddedToTheWidestCell() {
        List<String> headers = List.of("Id", "Name");
        List<List<String>> rows = List.of(
                List.of("1", "Alice"),
                List.of("2", "Bob"));

        // Column widths: max(header, every cell, 3) -> "Id"/"1"/"2" => 3, "Name"/"Alice"/"Bob" => 5.
        String expected = String.join("\n", List.of(
                "| " + padded("Id", 3) + " | " + padded("Name", 5) + " |",
                "| " + "-".repeat(3) + " | " + "-".repeat(5) + " |",
                "| " + padded("1", 3) + " | " + padded("Alice", 5) + " |",
                "| " + padded("2", 3) + " | " + padded("Bob", 5) + " |"));

        assertEquals(expected, MarkdownTableRenderer.render(headers, rows));
    }

    @Test
    void aColumnWiderThanThreeCharactersUsesAMatchingDashCount() {
        List<String> headers = List.of("Country");
        List<List<String>> rows = List.of(List.of("Germany"), List.of("US"));

        // Column width: max("Country"=7, "Germany"=7, "US"=2, 3) -> 7.
        String expected = String.join("\n", List.of(
                "| " + padded("Country", 7) + " |",
                "| " + "-".repeat(7) + " |",
                "| " + padded("Germany", 7) + " |",
                "| " + padded("US", 7) + " |"));

        assertEquals(expected, MarkdownTableRenderer.render(headers, rows));
    }

    @Test
    void rendersJustTheHeaderAndSeparatorWhenThereAreNoRows() {
        List<String> headers = List.of("Empty");

        String expected = String.join("\n", List.of(
                "| " + padded("Empty", 5) + " |",
                "| " + "-".repeat(5) + " |"));

        assertEquals(expected, MarkdownTableRenderer.render(headers, List.of()));
    }

    private static String padded(String cell, int width) {
        return cell + " ".repeat(width - cell.length());
    }
}
