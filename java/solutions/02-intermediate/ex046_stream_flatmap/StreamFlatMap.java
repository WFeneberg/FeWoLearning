package fewolearning.exercises.intermediate.ex046_stream_flatmap;

import java.util.List;
import java.util.stream.Collectors;

/*
Exercise 046 - Stream flatMap (reference solution).
*/
public final class StreamFlatMap {
    private StreamFlatMap() {
    }

    public static List<Integer> flatten(List<List<Integer>> nested) {
        return nested.stream()
                .flatMap(List::stream)
                .collect(Collectors.toList());
    }

    public static List<Character> allCharacters(List<String> words) {
        return words.stream()
                .flatMap(word -> word.chars().mapToObj(codePoint -> (char) codePoint))
                .collect(Collectors.toList());
    }
}
