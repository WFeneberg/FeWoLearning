package fewolearning.exercises.intermediate.ex048_sealed_ui_state

/*
Exercise 048 - Sealed UI state (intermediate).

Goal:   Render a label for each screen state using an exhaustive when.
Drills: state modeling, exhaustive when.
*/
sealed class UiState {
    object Loading : UiState()
    data class Loaded(val items: List<String>) : UiState()
    data class Error(val message: String) : UiState()
}

fun label(state: UiState): String {
    TODO()
}
