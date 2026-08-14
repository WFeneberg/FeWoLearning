package fewolearning.exercises.beginner.ex013_destructuring_pairs

/*
Exercise 013 - Destructuring pairs (reference solution).
*/
fun splitFullName(fullName: String): Pair<String, String> {
    val parts = fullName.trim().split(" ", limit = 2)
    return Pair(parts[0], parts.getOrElse(1) { "" })
}
