package fewolearning.exercises.beginner.ex001_primitive_math;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class PrimitiveMathTest {

    @Test
    void sumsTwoPositiveNumbers() {
        assertEquals(7, PrimitiveMath.sum(3, 4));
    }

    @Test
    void sumsWithANegativeNumber() {
        assertEquals(-1, PrimitiveMath.sum(-5, 4));
    }

    @Test
    void quotientTruncatesTowardZero() {
        assertEquals(3, PrimitiveMath.quotient(7, 2));
        assertEquals(-3, PrimitiveMath.quotient(-7, 2));
    }

    @Test
    void quotientThrowsOnDivisionByZero() {
        assertThrows(ArithmeticException.class, () -> PrimitiveMath.quotient(5, 0));
    }

    @Test
    void averageRoundedDownTruncatesTheFraction() {
        assertEquals(3, PrimitiveMath.averageRoundedDown(2, 3, 4));
        assertEquals(3, PrimitiveMath.averageRoundedDown(2, 3, 5));
    }
}
