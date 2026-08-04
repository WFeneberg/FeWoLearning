package fewolearning.exercises.advanced.ex084_lazy_thread_safety

/*
Exercise 084 - Lazy thread safety (advanced).

Goal:   Initialize a value exactly once even under concurrent first access.
Drills: lazy modes, concurrency tradeoffs.
*/
class SynchronizedLazyConfig(private val loader: () -> String) {
    val value: String by lazy(LazyThreadSafetyMode.SYNCHRONIZED) {
        TODO()
    }
}
