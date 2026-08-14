package fewolearning.exercises.advanced.ex083_dsl_marker_scope

/**
 * @DslMarker restricts implicit receiver access to the nearest enclosing block - a nested
 * lambda with its own @FormDsl-annotated receiver cannot implicitly call an outer FormBuilder's
 * members. That restriction is enforced by the compiler at the call site, so there is nothing to
 * assert about it at runtime here; this solution only implements normal, correctly-scoped usage.
 */
@DslMarker
annotation class FormDsl

@FormDsl
class FormBuilder {
    private val fields = mutableListOf<String>()

    fun field(name: String) {
        fields.add(name)
    }

    fun build(): List<String> = fields.toList()
}

fun form(block: FormBuilder.() -> Unit): List<String> {
    val builder = FormBuilder()
    builder.block()
    return builder.build()
}
