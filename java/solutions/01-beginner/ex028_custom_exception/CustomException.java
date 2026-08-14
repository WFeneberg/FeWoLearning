package fewolearning.exercises.beginner.ex028_custom_exception;

/*
Exercise 028 - Custom exception (reference solution).
*/
public final class CustomException {
    private CustomException() {
    }

    public static void requirePositive(int value) throws InvalidAmountException {
        if (value <= 0) {
            throw new InvalidAmountException("amount must be positive: " + value);
        }
    }

    public static final class InvalidAmountException extends Exception {
        public InvalidAmountException(String message) {
            super(message);
        }
    }
}
