package fewolearning.exercises.beginner.ex024_stream_grouping;

import org.junit.jupiter.api.Test;

import java.util.List;
import java.util.Map;

import static org.junit.jupiter.api.Assertions.assertEquals;

class StreamGroupingTest {

    @Test
    void groupByLengthGroupsWordsThatShareALength() {
        List<String> words = List.of("at", "on", "cat", "dog", "tree");

        Map<Integer, List<String>> grouped = StreamGrouping.groupByLength(words);

        assertEquals(Map.of(
                2, List.of("at", "on"),
                3, List.of("cat", "dog"),
                4, List.of("tree")
        ), grouped);
    }

    @Test
    void countByFirstLetterCountsWordsSharingAStartingLetter() {
        List<String> words = List.of("cat", "car", "dog", "deer", "cow");

        Map<Character, Long> counts = StreamGrouping.countByFirstLetter(words);

        assertEquals(Map.of('c', 3L, 'd', 2L), counts);
    }
}
