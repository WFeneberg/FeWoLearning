package fewolearning.exercises.advanced.ex089_result_pipeline

/** Parses a positive integer, wrapping both bad-format and non-positive failures in a Result. */
fun parsePositiveInt(rawValue: String): Result<Int> =
    runCatching { rawValue.toInt() }
        .mapCatching { parsed ->
            require(parsed > 0) { "must be positive: $parsed" }
            parsed
        }

/** Chains several Result-returning steps, short-circuiting via Result's own map combinator. */
fun pipeline(rawValue: String): Result<Int> =
    parsePositiveInt(rawValue)
        .map { it * 2 }
        .map { it + 1 }
