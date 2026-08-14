package fewolearning.exercises.intermediate.ex036_counter_state

/** Encapsulates a mutable, non-negative counter behind increment/decrement/reset. */
class CounterState {
    private var count: Int = 0

    fun increment(): Int {
        count += 1
        return count
    }

    fun decrement(): Int {
        if (count > 0) count -= 1
        return count
    }

    fun current(): Int = count

    fun reset() {
        count = 0
    }
}
