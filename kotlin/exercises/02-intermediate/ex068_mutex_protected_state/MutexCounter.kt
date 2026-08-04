package fewolearning.exercises.intermediate.ex068_mutex_protected_state

import kotlinx.coroutines.sync.Mutex

/*
Exercise 068 - Mutex protected state (intermediate).

Goal:   Guard a shared counter increment with a suspending Mutex.
Drills: Mutex, suspending critical sections.
*/
class MutexCounter {
    private val mutex = Mutex()
    private var count = 0

    suspend fun increment(): Int {
        TODO()
    }
}
