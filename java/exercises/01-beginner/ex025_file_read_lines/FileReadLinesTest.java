package fewolearning.exercises.beginner.ex025_file_read_lines;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;

class FileReadLinesTest {

    @Test
    void readLinesReturnsEveryLineInOrder(@TempDir Path tempDir) throws IOException {
        Path file = tempDir.resolve("notes.txt");
        Files.writeString(file, "first\nsecond\nthird\n", StandardCharsets.UTF_8);

        assertEquals(List.of("first", "second", "third"), FileReadLines.readLines(file));
    }

    @Test
    void countNonBlankLinesIgnoresBlankAndWhitespaceOnlyLines(@TempDir Path tempDir) throws IOException {
        Path file = tempDir.resolve("notes.txt");
        Files.writeString(file, "first\n\nsecond\n   \nthird\n", StandardCharsets.UTF_8);

        assertEquals(3, FileReadLines.countNonBlankLines(file));
    }
}
