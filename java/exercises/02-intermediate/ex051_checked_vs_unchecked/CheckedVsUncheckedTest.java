package fewolearning.exercises.intermediate.ex051_checked_vs_unchecked;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;
import static org.junit.jupiter.api.Assertions.assertThrows;

class CheckedVsUncheckedTest {

    @Test
    void withdrawSucceedsWhenBalanceCoversTheAmount() {
        assertDoesNotThrow(() -> CheckedVsUnchecked.withdraw(100.0, 40.0));
    }

    @Test
    void withdrawThrowsACheckedExceptionWhenFundsAreInsufficient() {
        assertThrows(CheckedVsUnchecked.InsufficientFundsException.class,
                () -> CheckedVsUnchecked.withdraw(30.0, 40.0));
    }

    @Test
    void withdrawThrowsAnUncheckedExceptionForANonPositiveAmount() {
        assertThrows(IllegalArgumentException.class,
                () -> CheckedVsUnchecked.withdraw(100.0, 0.0));
    }

    @Test
    void validateAmountAcceptsAPositiveAmount() {
        assertDoesNotThrow(() -> CheckedVsUnchecked.validateAmount(10.0));
    }

    @Test
    void validateAmountRejectsANegativeAmount() {
        assertThrows(IllegalArgumentException.class, () -> CheckedVsUnchecked.validateAmount(-5.0));
    }
}
