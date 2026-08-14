package fewolearning.exercises.beginner.ex025_file_read_lines;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;

/*
Exercise 025 - File read lines (reference solution).
*/
public final class FileReadLines {
    private FileReadLines() {
    }

    public static List<String> readLines(Path file) throws IOException {
        return Files.readAllLines(file, StandardCharsets.UTF_8);
    }

    public static long countNonBlankLines(Path file) throws IOException {
        return readLines(file).stream().filter(line -> !line.isBlank()).count();
    }
}
