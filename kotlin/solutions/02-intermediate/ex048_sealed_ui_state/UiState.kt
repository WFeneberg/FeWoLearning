package fewolearning.exercises.intermediate.ex048_sealed_ui_state

sealed class UiState {
    object Loading : UiState()
    data class Loaded(val items: List<String>) : UiState()
    data class Error(val message: String) : UiState()
}

/** Renders a human-readable label for each screen state. */
fun label(state: UiState): String = when (state) {
    is UiState.Loading -> "Loading"
    is UiState.Loaded -> "Loaded ${state.items.size} items"
    is UiState.Error -> "Error: ${state.message}"
}
