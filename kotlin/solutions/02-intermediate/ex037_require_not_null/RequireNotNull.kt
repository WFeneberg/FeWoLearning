package fewolearning.exercises.intermediate.ex037_require_not_null

/** Fails fast with a clear message when the required name is missing. */
fun requireName(name: String?): String = requireNotNull(name) { "name must not be null" }
