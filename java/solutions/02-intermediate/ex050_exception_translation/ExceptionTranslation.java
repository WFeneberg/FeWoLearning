package fewolearning.exercises.intermediate.ex050_exception_translation;

/*
Exercise 050 - Exception translation (reference solution).
*/
public final class ExceptionTranslation {
    private ExceptionTranslation() {
    }

    public static int parseConfigValue(String rawValue) {
        try {
            return Integer.parseInt(rawValue);
        } catch (NumberFormatException e) {
            throw new ConfigParseException("invalid config value: " + rawValue, e);
        }
    }

    public static final class ConfigParseException extends RuntimeException {
        public ConfigParseException(String message, Throwable cause) {
            super(message, cause);
        }
    }
}
