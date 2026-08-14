package fewolearning.exercises.advanced.ex084_lazy_thread_safety

/**
 * [LazyThreadSafetyMode.SYNCHRONIZED] (used here, and the default for `lazy { }`) guards
 * initialization with a lock, guaranteeing the loader runs exactly once even if multiple threads
 * race to read [value] first - at the cost of taking that lock on every read.
 * [LazyThreadSafetyMode.NONE] skips the lock entirely: fastest, but the loader could run more
 * than once (or corrupt state) under real concurrent first access, so it is only safe for
 * single-threaded use. [LazyThreadSafetyMode.PUBLICATION] allows the loader to run more than once
 * concurrently but publishes only the first result via an atomic compare-and-set, so no lock is
 * held but redundant computation can happen.
 */
class SynchronizedLazyConfig(private val loader: () -> String) {
    val value: String by lazy(LazyThreadSafetyMode.SYNCHRONIZED) {
        loader()
    }
}
