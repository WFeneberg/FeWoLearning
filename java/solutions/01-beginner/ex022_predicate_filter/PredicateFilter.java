package fewolearning.exercises.beginner.ex022_predicate_filter;

import java.util.ArrayList;
import java.util.List;
import java.util.function.Predicate;

/*
Exercise 022 - Predicate filter (reference solution).
*/
public final class PredicateFilter {
    private PredicateFilter() {
    }

    public static List<Integer> filter(List<Integer> numbers, Predicate<Integer> predicate) {
        List<Integer> matches = new ArrayList<>();
        for (Integer number : numbers) {
            if (predicate.test(number)) {
                matches.add(number);
            }
        }
        return matches;
    }

    public static int countNotMatching(List<Integer> numbers, Predicate<Integer> predicate) {
        int count = 0;
        for (Integer number : numbers) {
            if (!predicate.test(number)) {
                count++;
            }
        }
        return count;
    }
}
