package fewolearning.exercises.intermediate.ex046_variance_in

interface Consumer<in T> {
    fun consume(value: T)
}

/** Write-only, contravariant consumer that logs every value it receives. */
class LoggingConsumer<T> : Consumer<T> {
    val received = mutableListOf<Any?>()

    override fun consume(value: T) {
        received.add(value)
    }
}
