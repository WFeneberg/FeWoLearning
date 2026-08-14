package fewolearning.exercises.intermediate.ex041_inline_higher_order

/** Runs [block], swallowing any exception and returning null instead. */
inline fun <T> runOrNull(block: () -> T): T? =
    try {
        block()
    } catch (e: Exception) {
        null
    }
