package fewolearning.exercises.advanced.ex079_receiver_lambdas_html

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class SectionBuilderTest {

    @Test
    fun nestsAListBuilderInsideASectionBuilder() {
        val html = section {
            list {
                item("a")
                item("b")
            }
        }

        assertEquals("<ul><li>a</li><li>b</li></ul>", html)
    }
}
