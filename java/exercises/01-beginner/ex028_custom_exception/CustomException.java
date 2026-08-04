package fewolearning.exercises.beginner.ex028_custom_exception;

/*
Exercise 028 - Custom exception (beginner).

Goal:   Define a domain-specific exception and raise it with a clear message.
Drills: extending exceptions, meaningful messages.
*/
public final class CustomException {
    private CustomException() {
    }

    public static void requirePositive(int value) throws InvalidAmountException {
        throw new UnsupportedOperationException("TODO");
    }

    public static final class InvalidAmountException extends Exception {
        public InvalidAmountException(String message) {
            super(message);
        }
    }
}
