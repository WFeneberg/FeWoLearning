package fewolearning.exercises.beginner.ex005_nullable_basics

/*
Exercise 005 - Nullable basics (reference solution).
*/
fun describeLength(value: String?): String {
    return if (value != null) {
        "length: ${value.length}"
    } else {
        "no value"
    }
}
