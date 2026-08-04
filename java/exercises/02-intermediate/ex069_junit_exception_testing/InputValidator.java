package fewolearning.exercises.intermediate.ex069_junit_exception_testing;

/*
Exercise 069 - JUnit exception testing (intermediate).

Goal:   Validate input and fail fast with a descriptive message.
Drills: failure assertions, messages.
*/
public final class InputValidator {
    private InputValidator() {
    }

    public static void requireNonBlank(String value, String fieldName) {
        throw new UnsupportedOperationException("TODO");
    }
}
