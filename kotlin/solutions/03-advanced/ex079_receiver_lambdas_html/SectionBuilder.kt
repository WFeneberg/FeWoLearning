package fewolearning.exercises.advanced.ex079_receiver_lambdas_html

/** Nests a [ListBuilder] receiver lambda inside a [SectionBuilder] receiver lambda. */
class ListBuilder {
    private val items = mutableListOf<String>()

    fun item(text: String) {
        items.add("<li>$text</li>")
    }

    fun build(): String = "<ul>${items.joinToString(separator = "")}</ul>"
}

class SectionBuilder {
    private val parts = mutableListOf<String>()

    fun list(block: ListBuilder.() -> Unit) {
        val listBuilder = ListBuilder()
        listBuilder.block()
        parts.add(listBuilder.build())
    }

    fun build(): String = parts.joinToString(separator = "")
}

fun section(block: SectionBuilder.() -> Unit): String {
    val builder = SectionBuilder()
    builder.block()
    return builder.build()
}
