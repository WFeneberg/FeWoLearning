package fewolearning.exercises.advanced.ex074_flow_flatmap_latest

import kotlinx.coroutines.flow.Flow

/*
Exercise 074 - Flow flatMapLatest (advanced).

Goal:   Replace stale in-flight search results whenever a newer query arrives.
Drills: flatMapLatest, replacing stale work.
*/
fun latestResults(queries: Flow<String>, search: suspend (String) -> List<String>): Flow<List<String>> {
    TODO()
}
