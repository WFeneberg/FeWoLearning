package fewolearning.exercises.intermediate.ex037_require_not_null

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertThrows

class RequireNotNullTest {

    @Test
    fun returnsTheNameWhenItIsPresent() {
        assertEquals("Ada", requireName("Ada"))
    }

    @Test
    fun throwsWithAClearMessageWhenTheNameIsMissing() {
        val thrown = assertThrows(IllegalArgumentException::class.java) { requireName(null) }

        assertEquals(true, thrown.message?.isNotBlank())
    }
}
