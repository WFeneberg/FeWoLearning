package fewolearning.exercises.expert.ex093_rule_engine;

import java.util.List;
import java.util.NoSuchElementException;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class RuleEngineTest {

    @Test
    void returnsTheOutcomeOfTheFirstMatchingRuleInOrder() {
        RuleEngine<Integer> engine = new RuleEngine<>();
        List<RuleEngine.Rule<Integer>> rules = List.of(
                new RuleEngine.Rule<>(value -> value < 0, "negative"),
                new RuleEngine.Rule<>(value -> value == 0, "zero"),
                new RuleEngine.Rule<>(value -> value > 0, "positive"));

        assertEquals("zero", engine.evaluate(rules, 0));
        assertEquals("positive", engine.evaluate(rules, 5));
        assertEquals("negative", engine.evaluate(rules, -5));
    }

    @Test
    void firstMatchingRuleWinsWhenMultipleRulesWouldMatch() {
        RuleEngine<Integer> engine = new RuleEngine<>();
        List<RuleEngine.Rule<Integer>> rules = List.of(
                new RuleEngine.Rule<>(value -> value > 0, "positive"),
                new RuleEngine.Rule<>(value -> value > 10, "big positive"));

        assertEquals("positive", engine.evaluate(rules, 20));
    }

    @Test
    void throwsWhenNoRuleMatchesTheFact() {
        RuleEngine<Integer> engine = new RuleEngine<>();
        List<RuleEngine.Rule<Integer>> rules = List.of(
                new RuleEngine.Rule<>(value -> value < 0, "negative"));

        assertThrows(NoSuchElementException.class, () -> engine.evaluate(rules, 5));
    }
}
