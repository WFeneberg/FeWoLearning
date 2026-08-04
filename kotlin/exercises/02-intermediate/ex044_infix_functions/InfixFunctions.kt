package fewolearning.exercises.intermediate.ex044_infix_functions

/*
Exercise 044 - Infix functions (intermediate).

Goal:   Define an infix function that builds a labeled range-like pair.
Drills: infix notation, readability tradeoffs.
*/
data class Range(val start: Int, val end: Int)

infix fun Int.upTo(end: Int): Range {
    TODO()
}
