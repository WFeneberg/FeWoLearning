package fewolearning.exercises.intermediate.ex058_path_copy_move

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.io.TempDir
import java.nio.file.Files
import java.nio.file.Path

class PathCopyMoveTest {

    @Test
    fun copyOverwritingReplacesExistingTargetContent(@TempDir tempDir: Path) {
        val source = tempDir.resolve("source.txt")
        val target = tempDir.resolve("target.txt")
        Files.writeString(source, "fresh content")
        Files.writeString(target, "stale content")

        val result = copyOverwriting(source, target)

        assertEquals("fresh content", Files.readString(result))
        assertEquals("fresh content", Files.readString(target))
    }

    @Test
    fun copyOverwritingCreatesTheTargetWhenItDidNotExist(@TempDir tempDir: Path) {
        val source = tempDir.resolve("source.txt")
        val target = tempDir.resolve("new-target.txt")
        Files.writeString(source, "hello")

        copyOverwriting(source, target)

        assertEquals("hello", Files.readString(target))
    }
}
