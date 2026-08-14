package fewolearning.exercises.intermediate.ex068_mutex_protected_state

import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock

/** Guards a shared counter increment with a suspending Mutex. */
class MutexCounter {
    private val mutex = Mutex()
    private var count = 0

    suspend fun increment(): Int = mutex.withLock {
        count += 1
        count
    }
}
