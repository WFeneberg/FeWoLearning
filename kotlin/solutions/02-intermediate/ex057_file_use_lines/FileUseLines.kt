package fewolearning.exercises.intermediate.ex057_file_use_lines

import java.io.File

/** Counts non-blank lines in [file], closing the reader automatically via useLines. */
fun countNonBlankLines(file: File): Long =
    file.useLines { lines -> lines.count { it.isNotBlank() }.toLong() }
