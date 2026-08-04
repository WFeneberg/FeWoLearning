package fewolearning.exercises.intermediate.ex046_variance_in

/*
Exercise 046 - Contravariance (intermediate).

Goal:   Model a write-only, contravariant consumer.
Drills: contravariance, consumers.
*/
interface Consumer<in T> {
    fun consume(value: T)
}

class LoggingConsumer<T> : Consumer<T> {
    val received = mutableListOf<Any?>()

    override fun consume(value: T) {
        TODO()
    }
}
