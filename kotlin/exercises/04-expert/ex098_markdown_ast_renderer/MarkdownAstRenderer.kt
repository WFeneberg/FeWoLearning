package fewolearning.exercises.expert.ex098_markdown_ast_renderer

/*
Exercise 098 - Markdown AST renderer (expert).

Goal:   Render a small tree of Markdown nodes into a single output string.
Drills: tree traversal, rendering.
*/
sealed class MarkdownNode {
    data class Heading(val level: Int, val text: String) : MarkdownNode()
    data class Paragraph(val text: String) : MarkdownNode()
    data class Section(val children: List<MarkdownNode>) : MarkdownNode()
}

fun render(node: MarkdownNode): String {
    TODO()
}
