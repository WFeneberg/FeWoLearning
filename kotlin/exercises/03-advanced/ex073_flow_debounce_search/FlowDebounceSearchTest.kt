package fewolearning.exercises.advanced.ex073_flow_debounce_search

import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.toList
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class FlowDebounceSearchTest {

    @Test
    fun onlyTheQueryThatSettlesForTheFullWindowIsEmitted() = runTest {
        val queries = flow {
            emit("k")
            delay(50)
            emit("ko")
            delay(50)
            emit("kot")
            delay(300)
            emit("kotl")
            delay(300)
            emit("kotlin")
        }

        val settled = debouncedQueries(queries, debounceMillis = 200).toList()

        assertEquals(listOf("kot", "kotl", "kotlin"), settled)
    }
}
