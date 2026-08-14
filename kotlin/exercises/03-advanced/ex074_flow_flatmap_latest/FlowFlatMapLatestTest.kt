package fewolearning.exercises.advanced.ex074_flow_flatmap_latest

import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.toList
import kotlinx.coroutines.test.runTest
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertFalse

class FlowFlatMapLatestTest {

    @Test
    fun aSlowFirstSearchIsCancelledAndNeverAppearsInTheResults() = runTest {
        val queries = flow {
            emit("k")
            delay(300)
            emit("kotlin")
        }
        val search: suspend (String) -> List<String> = { query ->
            if (query == "k") {
                delay(1000)
                listOf("k-result-too-late")
            } else {
                delay(10)
                listOf("$query-result")
            }
        }

        val results = latestResults(queries, search).toList()

        assertEquals(listOf(listOf("kotlin-result")), results)
        assertFalse(results.any { it.contains("k-result-too-late") })
    }
}
