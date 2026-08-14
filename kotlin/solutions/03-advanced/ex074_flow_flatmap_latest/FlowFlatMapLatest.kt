package fewolearning.exercises.advanced.ex074_flow_flatmap_latest

import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.flatMapLatest
import kotlinx.coroutines.flow.flow

/** Cancels any in-flight search as soon as a newer query arrives, keeping only the latest result. */
@OptIn(ExperimentalCoroutinesApi::class)
fun latestResults(queries: Flow<String>, search: suspend (String) -> List<String>): Flow<List<String>> =
    queries.flatMapLatest { query -> flow { emit(search(query)) } }
