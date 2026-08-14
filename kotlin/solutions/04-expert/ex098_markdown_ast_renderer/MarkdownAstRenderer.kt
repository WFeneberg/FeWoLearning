package fewolearning.exercises.expert.ex098_markdown_ast_renderer

/**
 * Heading renders as level '#' characters + space + text; Paragraph renders as its text
 * alone; Section recursively renders each child and joins them with a blank line ("\n\n").
 */
sealed class MarkdownNode {
    data class Heading(val level: Int, val text: String) : MarkdownNode()
    data class Paragraph(val text: String) : MarkdownNode()
    data class Section(val children: List<MarkdownNode>) : MarkdownNode()
}

fun render(node: MarkdownNode): String = when (node) {
    is MarkdownNode.Heading -> "#".repeat(node.level) + " " + node.text
    is MarkdownNode.Paragraph -> node.text
    is MarkdownNode.Section -> node.children.joinToString("\n\n") { render(it) }
}
