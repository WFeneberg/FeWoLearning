package fewolearning.exercises.intermediate.ex063_nio_walk_files;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.Stream;

/*
Exercise 063 - NIO walk files (reference solution).
*/
public final class NioWalkFiles {
    private NioWalkFiles() {
    }

    public static List<Path> findByExtension(Path root, String extension) throws IOException {
        try (Stream<Path> paths = Files.walk(root)) {
            return paths.filter(Files::isRegularFile)
                    .filter(path -> path.getFileName().toString().endsWith(extension))
                    .collect(Collectors.toList());
        }
    }
}
