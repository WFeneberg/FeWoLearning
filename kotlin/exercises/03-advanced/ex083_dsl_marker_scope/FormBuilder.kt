package fewolearning.exercises.advanced.ex083_dsl_marker_scope

/*
Exercise 083 - DslMarker scope isolation (advanced).

Goal:   Prevent implicit access to an outer builder's receiver from a nested block.
Drills: @DslMarker, receiver isolation.
*/
@DslMarker
annotation class FormDsl

@FormDsl
class FormBuilder {
    private val fields = mutableListOf<String>()

    fun field(name: String) {
        TODO()
    }

    fun build(): List<String> {
        TODO()
    }
}

fun form(block: FormBuilder.() -> Unit): List<String> {
    TODO()
}
