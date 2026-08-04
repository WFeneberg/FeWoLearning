package fewolearning.exercises.expert.ex099_rules_engine

/*
Exercise 099 - Rules engine (expert).

Goal:   Evaluate prioritized rules against a fact and return the highest-priority match.
Drills: predicates, priorities, evaluation.
*/
data class Rule<T>(val priority: Int, val condition: (T) -> Boolean, val outcome: String)

fun <T> evaluate(rules: List<Rule<T>>, fact: T): String? {
    TODO()
}
