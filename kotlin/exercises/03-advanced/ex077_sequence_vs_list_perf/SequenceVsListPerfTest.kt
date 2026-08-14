package fewolearning.exercises.advanced.ex077_sequence_vs_list_perf

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertTrue

class SequenceVsListPerfTest {

    @Test
    fun bothPipelinesReturnTheSameEarlyMatch() {
        val numbers = (0 until 1000).toList()

        assertEquals(2, firstMatchEager(numbers) { it == 2 })
        assertEquals(2, firstMatchLazy(numbers) { it == 2 })
    }

    @Test
    fun theLazyPipelineInvokesThePredicateFarFewerTimesThanTheEagerPipeline() {
        val numbers = (0 until 1000).toList()

        var eagerCallCount = 0
        firstMatchEager(numbers) {
            eagerCallCount += 1
            it == 2
        }

        var lazyCallCount = 0
        firstMatchLazy(numbers) {
            lazyCallCount += 1
            it == 2
        }

        assertTrue(lazyCallCount < 10)
        assertTrue(eagerCallCount > 500)
        assertTrue(lazyCallCount < eagerCallCount)
    }
}
