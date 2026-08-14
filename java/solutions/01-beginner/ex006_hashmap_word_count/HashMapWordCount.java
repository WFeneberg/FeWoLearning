package fewolearning.exercises.beginner.ex006_hashmap_word_count;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

/*
Exercise 006 - HashMap word count (reference solution).
*/
public final class HashMapWordCount {
    private HashMapWordCount() {
    }

    public static Map<String, Integer> countWords(List<String> words) {
        Map<String, Integer> counts = new HashMap<>();
        for (String word : words) {
            counts.merge(word, 1, Integer::sum);
        }
        return counts;
    }

    public static int countWord(List<String> words, String targetWord) {
        return countWords(words).getOrDefault(targetWord, 0);
    }
}
