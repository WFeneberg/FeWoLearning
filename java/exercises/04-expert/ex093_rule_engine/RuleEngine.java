package fewolearning.exercises.expert.ex093_rule_engine;

import java.util.List;
import java.util.function.Predicate;

/*
Exercise 093 - Rule engine (expert).

Goal:   Evaluate an ordered set of rules against a fact, returning the first match.
Drills: predicates, composition, execution order.
*/
public final class RuleEngine<T> {
    public record Rule<T>(Predicate<T> condition, String outcome) {
    }

    public String evaluate(List<Rule<T>> rules, T fact) {
        throw new UnsupportedOperationException("TODO");
    }
}
