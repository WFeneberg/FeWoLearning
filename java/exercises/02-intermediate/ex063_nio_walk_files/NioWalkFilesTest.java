package fewolearning.exercises.intermediate.ex063_nio_walk_files;

import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

class NioWalkFilesTest {

    @Test
    void findsFilesMatchingTheExtensionAcrossNestedDirectories(@TempDir Path tempDir) throws Exception {
        Path nested = Files.createDirectories(tempDir.resolve("nested"));
        Path first = Files.createFile(tempDir.resolve("a.txt"));
        Path second = Files.createFile(nested.resolve("b.txt"));
        Files.createFile(tempDir.resolve("c.md"));

        List<Path> found = NioWalkFiles.findByExtension(tempDir, ".txt");

        assertEquals(2, found.size());
        assertTrue(found.contains(first));
        assertTrue(found.contains(second));
    }

    @Test
    void returnsAnEmptyListWhenNothingMatches(@TempDir Path tempDir) throws Exception {
        Files.createFile(tempDir.resolve("a.md"));

        List<Path> found = NioWalkFiles.findByExtension(tempDir, ".txt");

        assertTrue(found.isEmpty());
    }
}
