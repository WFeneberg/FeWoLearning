package fewolearning.exercises.intermediate.ex063_coroutine_supervisor

import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class CoroutineSupervisorTest {

    @Test
    fun oneFailingTaskDoesNotPreventTheOthersFromCompleting() = runTest {
        val completed = mutableListOf<Int>()
        val tasks = listOf<suspend () -> Unit>(
            { completed.add(0) },
            { throw RuntimeException("boom") },
            { completed.add(2) }
        )

        runIsolated(this, tasks)

        assertEquals(listOf(0, 2), completed)
    }
}
