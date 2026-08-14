package fewolearning.exercises.expert.ex091_coroutine_worker_pool

import java.util.Collections
import java.util.concurrent.atomic.AtomicInteger
import kotlinx.coroutines.channels.Channel
import kotlinx.coroutines.delay
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertTrue

class CoroutineWorkerPoolTest {

    @Test
    fun allTasksRunExactlyOnceAcrossGenuinelyOverlappingWorkers() = runTest {
        val workerCount = 3
        val taskCount = 9
        val ran = Collections.synchronizedList(mutableListOf<Int>())
        val active = AtomicInteger(0)
        val peakActive = AtomicInteger(0)

        // Buffered enough to hold every task so the test can send them all up front,
        // then close the channel so each worker's `for (task in tasks)` loop ends naturally.
        val channel = Channel<suspend () -> Unit>(capacity = taskCount)
        repeat(taskCount) { index ->
            channel.send {
                val current = active.incrementAndGet()
                peakActive.updateAndGet { prev -> maxOf(prev, current) }
                delay(10) // a real suspension point so overlapping workers are provable
                ran.add(index)
                active.decrementAndGet()
            }
        }
        channel.close()

        processAll(channel, workerCount)

        assertEquals((0 until taskCount).toList(), ran.sorted())
        // Peak concurrency must exceed 1: a sequential (single-worker-equivalent) implementation
        // could still run every task exactly once, so "ran all tasks" alone wouldn't prove
        // the bounded worker pool actually processes them concurrently.
        assertTrue(peakActive.get() > 1, "expected more than one worker to overlap, was ${peakActive.get()}")
        assertTrue(peakActive.get() <= workerCount, "peak concurrency should never exceed workerCount")
    }

    @Test
    fun handlesAnEmptyClosedChannelWithoutHanging() = runTest {
        val channel = Channel<suspend () -> Unit>(capacity = 1)
        channel.close()

        processAll(channel, workerCount = 4)
    }
}
