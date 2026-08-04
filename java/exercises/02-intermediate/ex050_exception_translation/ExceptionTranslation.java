package fewolearning.exercises.intermediate.ex050_exception_translation;

/*
Exercise 050 - Exception translation (intermediate).

Goal:   Translate a low-level NumberFormatException into a domain exception.
Drills: wrapping low-level failures.
*/
public final class ExceptionTranslation {
    private ExceptionTranslation() {
    }

    public static int parseConfigValue(String rawValue) {
        throw new UnsupportedOperationException("TODO");
    }

    public static final class ConfigParseException extends RuntimeException {
        public ConfigParseException(String message, Throwable cause) {
            super(message, cause);
        }
    }
}
