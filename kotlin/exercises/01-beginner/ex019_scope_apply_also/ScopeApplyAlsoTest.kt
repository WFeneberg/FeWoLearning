package fewolearning.exercises.beginner.ex019_scope_apply_also

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ScopeApplyAlsoTest {

    @Test
    fun buildConfiguredAppendsPrefixThenSuffix() {
        val result = buildConfigured("[", "]")

        assertEquals("[]", result.toString())
    }

    @Test
    fun buildConfiguredReturnsTheSameStringBuilderInstanceFromAlso() {
        val result = buildConfigured("<<", ">>")

        assertEquals("<<>>", result.toString())
    }
}
