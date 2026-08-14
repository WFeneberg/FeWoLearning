package fewolearning.exercises.expert.ex098_markdown_ast_renderer

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class MarkdownAstRendererTest {

    @Test
    fun rendersAHeadingWithHashesMatchingItsLevel() {
        assertEquals("### Title", render(MarkdownNode.Heading(3, "Title")))
    }

    @Test
    fun rendersAParagraphAsPlainText() {
        assertEquals("Just words.", render(MarkdownNode.Paragraph("Just words.")))
    }

    @Test
    fun rendersANestedSectionByRecursivelyRenderingAndJoiningItsChildren() {
        val section = MarkdownNode.Section(
            listOf(
                MarkdownNode.Heading(1, "Intro"),
                MarkdownNode.Paragraph("Some body text."),
                MarkdownNode.Section(
                    listOf(
                        MarkdownNode.Heading(2, "Sub"),
                        MarkdownNode.Paragraph("Nested text.")
                    )
                )
            )
        )

        val expected = "# Intro\n\nSome body text.\n\n## Sub\n\nNested text."

        assertEquals(expected, render(section))
    }
}
