package fewolearning.exercises.intermediate.ex057_file_use_lines

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.io.TempDir
import java.nio.file.Files
import java.nio.file.Path

class FileUseLinesTest {

    @Test
    fun countsOnlyTheNonBlankLines(@TempDir tempDir: Path) {
        val file = tempDir.resolve("lines.txt")
        Files.writeString(file, "first\n\nsecond\n   \nthird\n")

        val count = countNonBlankLines(file.toFile())

        assertEquals(3L, count)
    }

    @Test
    fun returnsZeroForAFileWithOnlyBlankLines(@TempDir tempDir: Path) {
        val file = tempDir.resolve("blank.txt")
        Files.writeString(file, "\n   \n\n")

        assertEquals(0L, countNonBlankLines(file.toFile()))
    }
}
