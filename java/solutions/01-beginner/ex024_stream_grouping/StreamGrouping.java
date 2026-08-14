package fewolearning.exercises.beginner.ex024_stream_grouping;

import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

/*
Exercise 024 - Stream grouping (reference solution).
*/
public final class StreamGrouping {
    private StreamGrouping() {
    }

    public static Map<Integer, List<String>> groupByLength(List<String> words) {
        return words.stream().collect(Collectors.groupingBy(String::length));
    }

    public static Map<Character, Long> countByFirstLetter(List<String> words) {
        return words.stream().collect(Collectors.groupingBy(word -> word.charAt(0), Collectors.counting()));
    }
}
