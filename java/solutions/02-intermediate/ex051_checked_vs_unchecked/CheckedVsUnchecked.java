package fewolearning.exercises.intermediate.ex051_checked_vs_unchecked;

/*
Exercise 051 - Checked vs unchecked (reference solution).
*/
public final class CheckedVsUnchecked {
    private CheckedVsUnchecked() {
    }

    public static void withdraw(double balance, double amount) throws InsufficientFundsException {
        validateAmount(amount);
        if (amount > balance) {
            throw new InsufficientFundsException(
                    "insufficient funds: balance=" + balance + ", requested=" + amount);
        }
    }

    public static void validateAmount(double amount) {
        if (amount <= 0) {
            throw new IllegalArgumentException("amount must be positive: " + amount);
        }
    }

    public static final class InsufficientFundsException extends Exception {
        public InsufficientFundsException(String message) {
            super(message);
        }
    }
}
