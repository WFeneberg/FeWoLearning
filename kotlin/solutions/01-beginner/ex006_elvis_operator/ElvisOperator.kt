package fewolearning.exercises.beginner.ex006_elvis_operator

/*
Exercise 006 - Elvis operator (reference solution).
*/
fun resolveName(name: String?): String = name?.takeIf { it.isNotBlank() } ?: "Anonymous"
