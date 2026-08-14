package fewolearning.exercises.advanced.ex082_sealed_error_hierarchy

sealed class DomainError {
    data class NotFound(val id: String) : DomainError()
    data class Validation(val field: String, val reason: String) : DomainError()
    data object Unauthorized : DomainError()
}

/** Maps each domain error variant to a distinct, user-facing message via an exhaustive when. */
fun userMessage(error: DomainError): String = when (error) {
    is DomainError.NotFound -> "Could not find item with id ${error.id}."
    is DomainError.Validation -> "Invalid ${error.field}: ${error.reason}."
    is DomainError.Unauthorized -> "You are not authorized to perform this action."
}
