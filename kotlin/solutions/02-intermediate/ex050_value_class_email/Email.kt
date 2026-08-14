package fewolearning.exercises.intermediate.ex050_value_class_email

/** Zero-overhead wrapper around a validated email string. */
@JvmInline
value class Email(val raw: String) {
    init {
        val parts = raw.split("@")
        require(parts.size == 2 && parts[0].isNotBlank() && parts[1].isNotBlank()) {
            "Invalid email: $raw"
        }
    }
}
