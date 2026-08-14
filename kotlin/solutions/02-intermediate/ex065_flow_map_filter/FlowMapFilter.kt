package fewolearning.exercises.intermediate.ex065_flow_map_filter

import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.filter
import kotlinx.coroutines.flow.map

/** Transforms a cold flow of numbers into a flow of even squares. */
fun evenSquaresFlow(source: Flow<Int>): Flow<Int> =
    source.filter { it % 2 == 0 }.map { it * it }
