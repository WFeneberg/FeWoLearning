package fewolearning.exercises.intermediate.ex046_stream_flatmap;

import java.util.List;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class StreamFlatMapTest {

    @Test
    void flattenCombinesNestedListsInOrder() {
        List<List<Integer>> nested = List.of(List.of(1, 2), List.of(3), List.of(4, 5));

        assertEquals(List.of(1, 2, 3, 4, 5), StreamFlatMap.flatten(nested));
    }

    @Test
    void flattenHandlesEmptyInnerLists() {
        List<List<Integer>> nested = List.of(List.of(), List.of(1), List.of());

        assertEquals(List.of(1), StreamFlatMap.flatten(nested));
    }

    @Test
    void allCharactersFlattensEachWordIntoItsCharacters() {
        List<String> words = List.of("ab", "c");

        assertEquals(List.of('a', 'b', 'c'), StreamFlatMap.allCharacters(words));
    }
}
