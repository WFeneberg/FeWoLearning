package fewolearning.exercises.expert.ex096_expression_evaluator

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ExpressionEvaluatorTest {

    @Test
    fun multiplicationBindsTighterThanAddition() {
        // A naive left-to-right (no precedence) evaluator would compute (2 + 3) * 4 = 20.
        assertEquals(14.0, evaluate("2 + 3 * 4"), 0.0001)
    }

    @Test
    fun parenthesesOverridePrecedence() {
        assertEquals(20.0, evaluate("(2 + 3) * 4"), 0.0001)
    }

    @Test
    fun supportsDivisionAndNestedParentheses() {
        assertEquals(3.0, evaluate("((6 + 4) / 2) - 2"), 0.0001)
    }

    @Test
    fun supportsUnaryMinus() {
        assertEquals(-1.0, evaluate("-3 + 2"), 0.0001)
    }

    @Test
    fun handlesDecimalLiterals() {
        assertEquals(2.5, evaluate("1.5 + 1"), 0.0001)
    }
}
