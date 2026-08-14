package fewolearning.exercises.expert.ex100_type_safe_sql_dsl

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class SelectBuilderTest {

    @Test
    fun buildsASelectStatementWithoutAWhereClause() {
        val sql = select("users") {
            column("id")
            column("name")
        }

        assertEquals("SELECT id, name FROM users", sql)
    }

    @Test
    fun appendsAWhereClauseWhenOneIsSet() {
        val sql = select("orders") {
            column("id")
            column("total")
            where("total > 100")
        }

        assertEquals("SELECT id, total FROM orders WHERE total > 100", sql)
    }

    @Test
    fun columnAndWhereReturnTheSameBuilderInstanceForChaining() {
        val builder = SelectBuilder("products")

        val chained = builder.column("id").column("price").where("price < 50")

        assertEquals(builder, chained)
        assertEquals("SELECT id, price FROM products WHERE price < 50", chained.build())
    }
}
