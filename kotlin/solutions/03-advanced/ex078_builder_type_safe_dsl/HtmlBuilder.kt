package fewolearning.exercises.advanced.ex078_builder_type_safe_dsl

/** Receiver-scoped builder that accumulates paragraph lines into a small HTML document. */
class HtmlBuilder {
    private val lines = mutableListOf<String>()

    fun paragraph(text: String) {
        lines.add("<p>$text</p>")
    }

    fun build(): String = lines.joinToString(separator = "\n")
}

fun html(block: HtmlBuilder.() -> Unit): String {
    val builder = HtmlBuilder()
    builder.block()
    return builder.build()
}
