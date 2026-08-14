package fewolearning.exercises.intermediate.ex068_mutex_protected_state

import kotlinx.coroutines.joinAll
import kotlinx.coroutines.launch
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class MutexCounterTest {

    @Test
    fun incrementIsExactUnderManyConcurrentCoroutines() = runTest {
        val counter = MutexCounter()

        val jobs = List(100) {
            launch {
                repeat(10) {
                    counter.increment()
                }
            }
        }
        jobs.joinAll()

        val finalValue = counter.increment()

        assertEquals(1001, finalValue)
    }
}
