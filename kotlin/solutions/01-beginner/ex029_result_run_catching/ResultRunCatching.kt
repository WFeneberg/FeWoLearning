package fewolearning.exercises.beginner.ex029_result_run_catching

/*
Exercise 029 - Result and runCatching (reference solution).
*/
fun parseOrDefault(rawValue: String, default: Int): Int =
    runCatching { rawValue.toInt() }.getOrDefault(default)
