package fewolearning.exercises.beginner.ex026_path_operations;

import java.nio.file.Path;

/*
Exercise 026 - Path operations (reference solution).
*/
public final class PathOperations {
    private PathOperations() {
    }

    public static Path resolveChild(Path baseDirectory, String childName) {
        return baseDirectory.resolve(childName);
    }

    public static Path normalize(Path path) {
        return path.normalize();
    }
}
