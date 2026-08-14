package fewolearning.exercises.intermediate.ex048_sealed_ui_state

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class UiStateTest {

    @Test
    fun labelDescribesTheLoadingState() {
        assertEquals("Loading", label(UiState.Loading))
    }

    @Test
    fun labelDescribesTheLoadedStateWithItsItemCount() {
        assertEquals("Loaded 3 items", label(UiState.Loaded(listOf("a", "b", "c"))))
    }

    @Test
    fun labelDescribesTheErrorStateWithItsMessage() {
        assertEquals("Error: network down", label(UiState.Error("network down")))
    }
}
