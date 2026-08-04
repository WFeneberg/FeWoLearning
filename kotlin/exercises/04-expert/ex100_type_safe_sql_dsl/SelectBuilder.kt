package fewolearning.exercises.expert.ex100_type_safe_sql_dsl

/*
Exercise 100 - Type-safe SQL DSL (expert).

Goal:   Build a SELECT statement from a table name and column list using a DSL.
Drills: builders, scope control, SQL rendering.
*/
class SelectBuilder(private val table: String) {
    private val columns = mutableListOf<String>()
    private var whereClause: String? = null

    fun column(name: String): SelectBuilder {
        TODO()
    }

    fun where(condition: String): SelectBuilder {
        TODO()
    }

    fun build(): String {
        TODO()
    }
}

fun select(table: String, block: SelectBuilder.() -> Unit): String {
    TODO()
}
