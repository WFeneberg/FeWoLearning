package fewolearning.exercises.advanced.ex085_java_interop_optionals

import java.util.Optional

/** Bridges a Java Optional into Kotlin's nullable convention. */
fun toNullable(optional: Optional<String>): String? = optional.orElse(null)

/** Bridges a Kotlin nullable value into Java's Optional convention. */
fun toOptional(value: String?): Optional<String> = Optional.ofNullable(value)
