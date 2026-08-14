package fewolearning.exercises.intermediate.ex069_test_dispatcher_time

import kotlinx.coroutines.delay

/** Delays for [delayMillis] then returns [value] — testable under virtual time. */
suspend fun delayedValue(delayMillis: Long, value: Int): Int {
    delay(delayMillis)
    return value
}
