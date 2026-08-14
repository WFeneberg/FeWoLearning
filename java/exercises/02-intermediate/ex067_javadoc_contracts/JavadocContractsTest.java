package fewolearning.exercises.intermediate.ex067_javadoc_contracts;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class JavadocContractsTest {

    @Test
    void divideTruncatesTowardZeroForANormalDivision() {
        assertEquals(3, JavadocContracts.divide(7, 2));
    }

    @Test
    void divideThrowsArithmeticExceptionWhenTheDivisorIsZero() {
        assertThrows(ArithmeticException.class, () -> JavadocContracts.divide(5, 0));
    }
}
