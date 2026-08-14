package fewolearning.exercises.beginner.ex020_string_builder_dsl

/*
Exercise 020 - StringBuilder DSL (reference solution).
*/
fun renderBulletList(items: List<String>): String = buildString {
    for (item in items) {
        append("- ")
        append(item)
        append("\n")
    }
}
