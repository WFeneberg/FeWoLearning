package fewolearning.exercises.intermediate.ex044_infix_functions

data class Range(val start: Int, val end: Int)

/** Builds a labeled [Range] from the receiver up to [end]. */
infix fun Int.upTo(end: Int): Range = Range(this, end)
