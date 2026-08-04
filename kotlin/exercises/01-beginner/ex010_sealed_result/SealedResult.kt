package fewolearning.exercises.beginner.ex010_sealed_result

/*
Exercise 010 - Sealed result (beginner).

Goal:   Model a parse outcome as a sealed hierarchy and describe it exhaustively.
Drills: sealed classes, exhaustive branching.
*/
sealed class ParseResult {
    data class Success(val value: Int) : ParseResult()
    data class Failure(val reason: String) : ParseResult()
}

fun describe(result: ParseResult): String {
    TODO()
}
