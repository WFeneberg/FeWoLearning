package fewolearning.exercises.beginner.ex024_stream_grouping;

import java.util.List;
import java.util.Map;

/*
Exercise 024 - Stream grouping (beginner).

Goal:   Group words by their length and count words per starting letter.
Drills: Collectors.groupingBy, aggregation.
*/
public final class StreamGrouping {
    private StreamGrouping() {
    }

    public static Map<Integer, List<String>> groupByLength(List<String> words) {
        throw new UnsupportedOperationException("TODO");
    }

    public static Map<Character, Long> countByFirstLetter(List<String> words) {
        throw new UnsupportedOperationException("TODO");
    }
}
