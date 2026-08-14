package fewolearning.exercises.expert.ex099_rules_engine

/**
 * Scans a priority-descending COPY of [rules] (never mutates or trusts the caller's
 * ordering) and returns the first match's outcome, or null if none match.
 */
data class Rule<T>(val priority: Int, val condition: (T) -> Boolean, val outcome: String)

fun <T> evaluate(rules: List<Rule<T>>, fact: T): String? =
    rules.sortedByDescending { it.priority }.firstOrNull { it.condition(fact) }?.outcome
