package fewolearning.exercises.beginner.ex019_scope_apply_also

/*
Exercise 019 - Scope functions apply/also (reference solution).
*/
fun buildConfigured(prefix: String, suffix: String): StringBuilder =
    StringBuilder()
        .apply {
            append(prefix)
            append(suffix)
        }
        .also { println("Built: $it") }
