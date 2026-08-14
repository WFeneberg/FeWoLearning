package fewolearning.exercises.beginner.ex026_reified_type_check

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertTrue
import org.junit.jupiter.api.Assertions.assertFalse

class ReifiedTypeCheckTest {

    @Test
    fun isInstanceOfIsTrueWhenTheValueMatchesTheReifiedType() {
        assertTrue(isInstanceOf<String>("hello"))
        assertTrue(isInstanceOf<Int>(42))
    }

    @Test
    fun isInstanceOfIsFalseWhenTheValueDoesNotMatchTheReifiedType() {
        assertFalse(isInstanceOf<String>(42))
        assertFalse(isInstanceOf<Int>("hello"))
    }
}
