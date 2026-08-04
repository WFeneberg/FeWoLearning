package fewolearning.exercises.advanced.ex088_map_get_or_put_cache

/*
Exercise 088 - Map getOrPut cache (advanced).

Goal:   Cache expensive computations per key using getOrPut.
Drills: caching, mutation, thread-safety caveats.
*/
class ComputationCache {
    private val cache = mutableMapOf<Int, Int>()

    fun computeOrCached(key: Int, compute: (Int) -> Int): Int {
        TODO()
    }
}
