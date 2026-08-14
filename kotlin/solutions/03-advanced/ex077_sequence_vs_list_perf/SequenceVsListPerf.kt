package fewolearning.exercises.advanced.ex077_sequence_vs_list_perf

/** Eagerly maps then filters the whole list before finding the first match. */
fun firstMatchEager(numbers: List<Int>, predicate: (Int) -> Boolean): Int? =
    numbers.map { it }.filter(predicate).firstOrNull()

/** Lazily evaluates the pipeline element-by-element, stopping as soon as a match is found. */
fun firstMatchLazy(numbers: List<Int>, predicate: (Int) -> Boolean): Int? =
    numbers.asSequence().map { it }.filter(predicate).firstOrNull()
