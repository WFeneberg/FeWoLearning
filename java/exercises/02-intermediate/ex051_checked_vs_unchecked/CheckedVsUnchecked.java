package fewolearning.exercises.intermediate.ex051_checked_vs_unchecked;

/*
Exercise 051 - Checked vs unchecked (intermediate).

Goal:   Model a recoverable checked failure and a programmer-error unchecked one.
Drills: exception design, API tradeoffs.
*/
public final class CheckedVsUnchecked {
    private CheckedVsUnchecked() {
    }

    public static void withdraw(double balance, double amount) throws InsufficientFundsException {
        throw new UnsupportedOperationException("TODO");
    }

    public static void validateAmount(double amount) {
        throw new UnsupportedOperationException("TODO");
    }

    public static final class InsufficientFundsException extends Exception {
        public InsufficientFundsException(String message) {
            super(message);
        }
    }
}
