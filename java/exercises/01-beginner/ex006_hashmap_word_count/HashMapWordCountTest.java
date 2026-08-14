package fewolearning.exercises.beginner.ex006_hashmap_word_count;

import org.junit.jupiter.api.Test;

import java.util.List;
import java.util.Map;

import static org.junit.jupiter.api.Assertions.assertEquals;

class HashMapWordCountTest {

    @Test
    void countWordsCountsEachDistinctWord() {
        List<String> words = List.of("apple", "banana", "apple", "cherry", "banana", "apple");

        Map<String, Integer> counts = HashMapWordCount.countWords(words);

        assertEquals(Map.of("apple", 3, "banana", 2, "cherry", 1), counts);
    }

    @Test
    void countWordsOnEmptyListIsEmptyMap() {
        assertEquals(Map.of(), HashMapWordCount.countWords(List.of()));
    }

    @Test
    void countWordCountsOccurrencesOfOneTargetWord() {
        List<String> words = List.of("apple", "banana", "apple", "cherry", "apple");

        assertEquals(3, HashMapWordCount.countWord(words, "apple"));
    }

    @Test
    void countWordIsZeroWhenWordIsAbsent() {
        List<String> words = List.of("apple", "banana");

        assertEquals(0, HashMapWordCount.countWord(words, "cherry"));
    }
}
