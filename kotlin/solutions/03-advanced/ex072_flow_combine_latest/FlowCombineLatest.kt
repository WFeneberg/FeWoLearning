package fewolearning.exercises.advanced.ex072_flow_combine_latest

import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.combine

/** Combines the latest values from both flows into a paired flow using the stdlib [combine] operator. */
fun combinePairs(first: Flow<Int>, second: Flow<String>): Flow<Pair<Int, String>> =
    combine(first, second) { a, b -> a to b }
