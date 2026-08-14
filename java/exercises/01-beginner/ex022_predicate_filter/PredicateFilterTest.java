package fewolearning.exercises.beginner.ex022_predicate_filter;

import org.junit.jupiter.api.Test;

import java.util.List;
import java.util.function.Predicate;

import static org.junit.jupiter.api.Assertions.assertEquals;

class PredicateFilterTest {

    private static final Predicate<Integer> IS_EVEN = value -> value % 2 == 0;

    @Test
    void filterKeepsOnlyMatchingElements() {
        List<Integer> numbers = List.of(1, 2, 3, 4, 5, 6);

        assertEquals(List.of(2, 4, 6), PredicateFilter.filter(numbers, IS_EVEN));
    }

    @Test
    void filterCanReturnAnEmptyListWhenNothingMatches() {
        List<Integer> numbers = List.of(1, 3, 5);

        assertEquals(List.of(), PredicateFilter.filter(numbers, IS_EVEN));
    }

    @Test
    void countNotMatchingCountsElementsFailingThePredicate() {
        List<Integer> numbers = List.of(1, 2, 3, 4, 5, 6);

        assertEquals(3, PredicateFilter.countNotMatching(numbers, IS_EVEN));
    }
}
