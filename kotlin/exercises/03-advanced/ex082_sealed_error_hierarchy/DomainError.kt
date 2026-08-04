package fewolearning.exercises.advanced.ex082_sealed_error_hierarchy

/*
Exercise 082 - Sealed error hierarchy (advanced).

Goal:   Map each domain error variant to a user-facing message.
Drills: rich domain errors, matching.
*/
sealed class DomainError {
    data class NotFound(val id: String) : DomainError()
    data class Validation(val field: String, val reason: String) : DomainError()
    data object Unauthorized : DomainError()
}

fun userMessage(error: DomainError): String {
    TODO()
}
