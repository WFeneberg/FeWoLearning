package fewolearning.exercises.intermediate.ex040_sequence_pipeline

/** Lazily filters evens, squares them, and materializes only the first [count]. */
fun firstEvenSquares(numbers: List<Int>, count: Int): List<Int> =
    numbers.asSequence()
        .filter { it % 2 == 0 }
        .map { it * it }
        .take(count)
        .toList()
