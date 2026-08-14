package fewolearning.exercises.expert.ex100_type_safe_sql_dsl

/**
 * build() renders "SELECT c1, c2 FROM table", appending " WHERE ..." only when where()
 * was called. column/where mutate internal state and return `this` so calls chain.
 */
class SelectBuilder(private val table: String) {
    private val columns = mutableListOf<String>()
    private var whereClause: String? = null

    fun column(name: String): SelectBuilder {
        columns.add(name)
        return this
    }

    fun where(condition: String): SelectBuilder {
        whereClause = condition
        return this
    }

    fun build(): String {
        val base = "SELECT ${columns.joinToString(", ")} FROM $table"
        return whereClause?.let { "$base WHERE $it" } ?: base
    }
}

fun select(table: String, block: SelectBuilder.() -> Unit): String {
    val builder = SelectBuilder(table)
    builder.block()
    return builder.build()
}
