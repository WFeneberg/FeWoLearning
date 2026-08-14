package fewolearning.exercises.expert.ex093_rule_engine;

import java.util.List;
import java.util.NoSuchElementException;
import java.util.function.Predicate;

/*
Exercise 093 - Rule engine (reference solution).
*/
public final class RuleEngine<T> {
    public record Rule<T>(Predicate<T> condition, String outcome) {
    }

    public String evaluate(List<Rule<T>> rules, T fact) {
        for (Rule<T> rule : rules) {
            if (rule.condition().test(fact)) {
                return rule.outcome();
            }
        }
        throw new NoSuchElementException("No rule matched the given fact");
    }
}
