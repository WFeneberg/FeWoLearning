package fewolearning.exercises.advanced.ex088_map_get_or_put_cache

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ComputationCacheTest {

    @Test
    fun computeIsInvokedExactlyOncePerDistinctKeyAcrossMultipleCalls() {
        val cache = ComputationCache()
        var invocationCount = 0
        val compute: (Int) -> Int = { key ->
            invocationCount += 1
            key * key
        }

        val firstCallForFive = cache.computeOrCached(5, compute)
        val secondCallForFive = cache.computeOrCached(5, compute)
        val callForSeven = cache.computeOrCached(7, compute)

        assertEquals(25, firstCallForFive)
        assertEquals(25, secondCallForFive)
        assertEquals(49, callForSeven)
        assertEquals(2, invocationCount)
    }
}
