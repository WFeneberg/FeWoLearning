package fewolearning.exercises.beginner.ex003_ranges_and_when

/*
Exercise 003 - Ranges and when (reference solution).
*/
fun classify(score: Int): String = when (score) {
    in 90..100 -> "A"
    in 80..89 -> "B"
    in 70..79 -> "C"
    in 60..69 -> "D"
    else -> "F"
}

fun isInRange(value: Int, range: IntRange): Boolean = value in range
