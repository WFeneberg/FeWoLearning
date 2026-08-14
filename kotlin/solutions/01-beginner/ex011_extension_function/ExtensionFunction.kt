package fewolearning.exercises.beginner.ex011_extension_function

/*
Exercise 011 - Extension function (reference solution).
*/
fun String.isPalindrome(): Boolean {
    val normalized = this.lowercase().filter { it.isLetterOrDigit() }
    return normalized == normalized.reversed()
}
