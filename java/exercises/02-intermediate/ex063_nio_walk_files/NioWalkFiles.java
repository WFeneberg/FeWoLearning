package fewolearning.exercises.intermediate.ex063_nio_walk_files;

import java.io.IOException;
import java.nio.file.Path;
import java.util.List;

/*
Exercise 063 - NIO walk files (intermediate).

Goal:   Walk a directory tree and collect paths matching a file extension.
Drills: Files.walk, filtering paths.
*/
public final class NioWalkFiles {
    private NioWalkFiles() {
    }

    public static List<Path> findByExtension(Path root, String extension) throws IOException {
        throw new UnsupportedOperationException("TODO");
    }
}
