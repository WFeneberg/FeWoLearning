package fewolearning.exercises.intermediate.ex056_exception_wrapping

/*
Exercise 056 - Exception wrapping (intermediate).

Goal:   Wrap a low-level NumberFormatException into a domain exception with a cause.
Drills: domain exceptions, causes.
*/
class ConfigParseException(message: String, cause: Throwable) : RuntimeException(message, cause)

fun parseConfigValue(rawValue: String): Int {
    TODO()
}
