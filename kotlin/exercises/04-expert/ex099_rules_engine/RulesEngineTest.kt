package fewolearning.exercises.expert.ex099_rules_engine

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertNull

class RulesEngineTest {

    @Test
    fun returnsTheOutcomeOfTheHighestPriorityMatchingRuleEvenWhenGivenOutOfOrder() {
        // Deliberately not given in priority order, to prove evaluate sorts rather than
        // trusting (or coincidentally matching) the caller's list order.
        val rules = listOf(
            Rule<Int>(priority = 1, condition = { it > 0 }, outcome = "low-priority-positive"),
            Rule<Int>(priority = 10, condition = { it > 0 }, outcome = "high-priority-positive"),
            Rule<Int>(priority = 5, condition = { it > 100 }, outcome = "mid-priority-large")
        )

        assertEquals("high-priority-positive", evaluate(rules, 5))
    }

    @Test
    fun fallsThroughToALowerPriorityRuleWhenTheHighestPriorityRuleDoesNotMatch() {
        val rules = listOf(
            Rule<Int>(priority = 10, condition = { it > 100 }, outcome = "high"),
            Rule<Int>(priority = 1, condition = { it > 0 }, outcome = "low")
        )

        assertEquals("low", evaluate(rules, 5))
    }

    @Test
    fun returnsNullWhenNoRuleMatches() {
        val rules = listOf(Rule<Int>(priority = 1, condition = { it > 100 }, outcome = "only"))

        assertNull(evaluate(rules, 5))
    }
}
