package fewolearning.exercises.intermediate.ex055_result_recover

/** Recovers a failed Result with a fallback [default] value. */
fun <T> Result<T>.recoverWithDefault(default: T): T = this.getOrElse { default }
