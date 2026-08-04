package fewolearning.exercises.intermediate.ex045_variance_out

/*
Exercise 045 - Declaration-site covariance (intermediate).

Goal:   Model a read-only, covariant producer of values.
Drills: declaration-site covariance.
*/
interface Producer<out T> {
    fun produce(): T
}

class ValueProducer<T>(private val value: T) : Producer<T> {
    override fun produce(): T {
        TODO()
    }
}
