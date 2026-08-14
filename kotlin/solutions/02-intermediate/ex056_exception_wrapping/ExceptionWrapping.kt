package fewolearning.exercises.intermediate.ex056_exception_wrapping

class ConfigParseException(message: String, cause: Throwable) : RuntimeException(message, cause)

/** Parses a config value, wrapping any low-level parse failure with its cause. */
fun parseConfigValue(rawValue: String): Int =
    try {
        rawValue.toInt()
    } catch (e: NumberFormatException) {
        throw ConfigParseException("Invalid config value: $rawValue", e)
    }
