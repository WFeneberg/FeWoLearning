package fewolearning.exercises.intermediate.ex061_coroutine_launch_join

import kotlinx.coroutines.delay
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class CoroutineLaunchJoinTest {

    @Test
    fun runBothAndWaitCompletesBothChildrenBeforeReturning() = runTest {
        val completed = mutableListOf<String>()

        runBothAndWait(
            this,
            first = {
                delay(100)
                completed.add("first")
            },
            second = {
                delay(50)
                completed.add("second")
            }
        )

        assertEquals(setOf("first", "second"), completed.toSet())
        assertEquals(2, completed.size)
    }
}
