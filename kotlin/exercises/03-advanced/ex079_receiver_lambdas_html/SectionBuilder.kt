package fewolearning.exercises.advanced.ex079_receiver_lambdas_html

/*
Exercise 079 - Receiver lambdas HTML (advanced).

Goal:   Nest a list builder inside a section builder using receiver lambdas.
Drills: nested receivers, mini DSL.
*/
class ListBuilder {
    private val items = mutableListOf<String>()

    fun item(text: String) {
        TODO()
    }

    fun build(): String {
        TODO()
    }
}

class SectionBuilder {
    private val parts = mutableListOf<String>()

    fun list(block: ListBuilder.() -> Unit) {
        TODO()
    }

    fun build(): String {
        TODO()
    }
}

fun section(block: SectionBuilder.() -> Unit): String {
    TODO()
}
