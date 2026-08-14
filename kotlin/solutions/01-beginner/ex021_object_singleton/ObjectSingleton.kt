package fewolearning.exercises.beginner.ex021_object_singleton

/*
Exercise 021 - Object singleton (reference solution).
*/
object RequestCounter {
    private var count: Int = 0

    fun increment(): Int {
        count += 1
        return count
    }

    fun current(): Int = count
}
