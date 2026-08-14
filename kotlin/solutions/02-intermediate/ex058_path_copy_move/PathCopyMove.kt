package fewolearning.exercises.intermediate.ex058_path_copy_move

import java.nio.file.Files
import java.nio.file.Path
import java.nio.file.StandardCopyOption

/** Copies [source] to [target], overwriting any existing file at [target]. */
fun copyOverwriting(source: Path, target: Path): Path =
    Files.copy(source, target, StandardCopyOption.REPLACE_EXISTING)
