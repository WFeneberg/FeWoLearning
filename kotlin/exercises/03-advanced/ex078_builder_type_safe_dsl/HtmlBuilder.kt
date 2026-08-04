package fewolearning.exercises.advanced.ex078_builder_type_safe_dsl

/*
Exercise 078 - Type-safe builder DSL (advanced).

Goal:   Build an Html document using a receiver-scoped builder function.
Drills: receivers, fluent builders.
*/
class HtmlBuilder {
    private val lines = mutableListOf<String>()

    fun paragraph(text: String) {
        TODO()
    }

    fun build(): String {
        TODO()
    }
}

fun html(block: HtmlBuilder.() -> Unit): String {
    TODO()
}
