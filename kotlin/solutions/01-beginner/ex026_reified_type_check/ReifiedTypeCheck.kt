package fewolearning.exercises.beginner.ex026_reified_type_check

/*
Exercise 026 - Reified type check (reference solution).
*/
inline fun <reified T> isInstanceOf(value: Any?): Boolean = value is T
