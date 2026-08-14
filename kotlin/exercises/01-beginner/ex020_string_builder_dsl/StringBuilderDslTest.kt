package fewolearning.exercises.beginner.ex020_string_builder_dsl

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class StringBuilderDslTest {

    @Test
    fun renderBulletListPrefixesEachItemWithADash() {
        assertEquals("- a\n- b\n", renderBulletList(listOf("a", "b")))
    }

    @Test
    fun renderBulletListReturnsAnEmptyStringForNoItems() {
        assertEquals("", renderBulletList(emptyList()))
    }
}
