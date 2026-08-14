package fewolearning.exercises.advanced.ex088_map_get_or_put_cache

/** Caches each key's computed result the first time it is requested, via getOrPut. */
class ComputationCache {
    private val cache = mutableMapOf<Int, Int>()

    fun computeOrCached(key: Int, compute: (Int) -> Int): Int =
        cache.getOrPut(key) { compute(key) }
}
