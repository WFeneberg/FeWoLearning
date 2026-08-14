package fewolearning.exercises.beginner.ex018_scope_let_run

/*
Exercise 018 - Scope functions let/run (reference solution).
*/
fun describeOrDefault(value: String?, default: String): String =
    value?.let { "Value: $it" } ?: default

fun computeArea(width: Int, height: Int): Int = width.run { this * height }
