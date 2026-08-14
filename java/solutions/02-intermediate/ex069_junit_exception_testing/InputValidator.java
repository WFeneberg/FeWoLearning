package fewolearning.exercises.intermediate.ex069_junit_exception_testing;

/*
Exercise 069 - JUnit exception testing (reference solution).
*/
public final class InputValidator {
    private InputValidator() {
    }

    public static void requireNonBlank(String value, String fieldName) {
        if (value == null || value.isBlank()) {
            throw new IllegalArgumentException(fieldName + " must not be blank");
        }
    }
}
