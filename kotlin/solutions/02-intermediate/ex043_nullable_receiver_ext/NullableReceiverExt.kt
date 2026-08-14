package fewolearning.exercises.intermediate.ex043_nullable_receiver_ext

/** Treats a null or blank receiver the same way, falling back to [default]. */
fun String?.orDefault(default: String): String =
    if (this.isNullOrBlank()) default else this
