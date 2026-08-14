package fewolearning.exercises.intermediate.ex055_result_recover

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ResultRecoverTest {

    @Test
    fun successfulResultKeepsItsOwnValue() {
        val result: Result<Int> = Result.success(5)

        assertEquals(5, result.recoverWithDefault(0))
    }

    @Test
    fun failedResultFallsBackToTheDefault() {
        val result: Result<Int> = Result.failure(RuntimeException("boom"))

        assertEquals(0, result.recoverWithDefault(0))
    }
}
