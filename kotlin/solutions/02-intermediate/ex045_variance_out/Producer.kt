package fewolearning.exercises.intermediate.ex045_variance_out

interface Producer<out T> {
    fun produce(): T
}

/** Read-only, covariant producer of a fixed [value]. */
class ValueProducer<T>(private val value: T) : Producer<T> {
    override fun produce(): T = value
}
