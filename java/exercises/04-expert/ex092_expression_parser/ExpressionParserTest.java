package fewolearning.exercises.expert.ex092_expression_parser;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class ExpressionParserTest {

    private static final double DELTA = 1e-9;

    @Test
    void multiplicationBindsTighterThanAddition() {
        assertEquals(14.0, ExpressionParser.evaluate("2 + 3 * 4"), DELTA);
    }

    @Test
    void parenthesesOverridePrecedence() {
        assertEquals(20.0, ExpressionParser.evaluate("(2 + 3) * 4"), DELTA);
    }

    @Test
    void divisionAndSubtractionEvaluateLeftToRight() {
        assertEquals(2.0, ExpressionParser.evaluate("10 / 2 - 3"), DELTA);
    }

    @Test
    void supportsNestedParenthesesAndUnaryMinus() {
        assertEquals(12.0, ExpressionParser.evaluate("2 * (3 + (4 - 1))"), DELTA);
        assertEquals(1.0, ExpressionParser.evaluate("-3 + 4"), DELTA);
    }

    @Test
    void rejectsATrailingOperatorWithNoOperand() {
        assertThrows(IllegalArgumentException.class, () -> ExpressionParser.evaluate("2 +"));
    }

    @Test
    void rejectsUnbalancedParentheses() {
        assertThrows(IllegalArgumentException.class, () -> ExpressionParser.evaluate("(2 + 3"));
    }
}
