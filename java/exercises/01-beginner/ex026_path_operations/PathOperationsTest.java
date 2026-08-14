package fewolearning.exercises.beginner.ex026_path_operations;

import org.junit.jupiter.api.Test;

import java.nio.file.Path;

import static org.junit.jupiter.api.Assertions.assertEquals;

class PathOperationsTest {

    @Test
    void resolveChildAppendsTheChildNameToTheBaseDirectory() {
        Path base = Path.of("reports", "2026");

        assertEquals(Path.of("reports", "2026", "august.txt"), PathOperations.resolveChild(base, "august.txt"));
    }

    @Test
    void normalizeCollapsesDotDotSegments() {
        Path messy = Path.of("reports", "2026", "..", "2025", "august.txt");

        assertEquals(Path.of("reports", "2025", "august.txt"), PathOperations.normalize(messy));
    }
}
