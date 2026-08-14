package fewolearning.exercises.intermediate.ex038_collection_group_by

/** Groups words by length, then counts how many words landed in each group. */
fun countByLength(words: List<String>): Map<Int, Int> =
    words.groupBy { it.length }.mapValues { it.value.size }
