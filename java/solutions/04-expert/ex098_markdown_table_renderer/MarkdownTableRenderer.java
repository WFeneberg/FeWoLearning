package fewolearning.exercises.expert.ex098_markdown_table_renderer;

import java.util.ArrayList;
import java.util.List;

/*
Exercise 098 - Markdown table renderer (reference solution).

Column width = the widest of the header, every row's cell in that column, and
a minimum of 3 characters (so the "---" separator is never truncated). Cells
are left-aligned and padded with spaces; the separator row uses exactly as
many dashes as the column width. Rows are joined with "\n" and there is no
trailing newline after the last row.
*/
public final class MarkdownTableRenderer {
    private MarkdownTableRenderer() {
    }

    public static String render(List<String> headers, List<List<String>> rows) {
        int columnCount = headers.size();
        int[] widths = new int[columnCount];
        for (int column = 0; column < columnCount; column++) {
            widths[column] = Math.max(3, headers.get(column).length());
        }
        for (List<String> row : rows) {
            for (int column = 0; column < columnCount; column++) {
                widths[column] = Math.max(widths[column], row.get(column).length());
            }
        }

        List<String> lines = new ArrayList<>();
        lines.add(renderRow(headers, widths));
        lines.add(renderSeparator(widths));
        for (List<String> row : rows) {
            lines.add(renderRow(row, widths));
        }
        return String.join("\n", lines);
    }

    private static String renderRow(List<String> cells, int[] widths) {
        StringBuilder builder = new StringBuilder("|");
        for (int column = 0; column < cells.size(); column++) {
            builder.append(' ').append(pad(cells.get(column), widths[column])).append(" |");
        }
        return builder.toString();
    }

    private static String renderSeparator(int[] widths) {
        StringBuilder builder = new StringBuilder("|");
        for (int width : widths) {
            builder.append(' ').append("-".repeat(width)).append(" |");
        }
        return builder.toString();
    }

    private static String pad(String cell, int width) {
        StringBuilder padded = new StringBuilder(cell);
        while (padded.length() < width) {
            padded.append(' ');
        }
        return padded.toString();
    }
}
