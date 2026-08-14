package fewolearning.exercises.beginner.ex010_sealed_result

/*
Exercise 010 - Sealed result (reference solution).
*/
sealed class ParseResult {
    data class Success(val value: Int) : ParseResult()
    data class Failure(val reason: String) : ParseResult()
}

fun describe(result: ParseResult): String = when (result) {
    is ParseResult.Success -> "Success: ${result.value}"
    is ParseResult.Failure -> "Failure: ${result.reason}"
}
