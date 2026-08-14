package fewolearning.exercises.advanced.ex073_flow_debounce_search

import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.debounce

/** Emits only a query that settles for [debounceMillis] without a newer one arriving. */
fun debouncedQueries(queries: Flow<String>, debounceMillis: Long): Flow<String> =
    queries.debounce(debounceMillis)
