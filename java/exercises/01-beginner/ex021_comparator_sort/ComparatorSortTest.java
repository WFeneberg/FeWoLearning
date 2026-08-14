package fewolearning.exercises.beginner.ex021_comparator_sort;

import org.junit.jupiter.api.Test;

import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;

class ComparatorSortTest {

    @Test
    void sortByLengthOrdersShortestFirst() {
        List<String> names = List.of("Beatrice", "Al", "Cara");

        assertEquals(List.of("Al", "Cara", "Beatrice"), ComparatorSort.sortByLength(names));
    }

    @Test
    void sortByLengthDoesNotMutateTheInputList() {
        List<String> names = List.of("Beatrice", "Al", "Cara");

        ComparatorSort.sortByLength(names);

        assertEquals(List.of("Beatrice", "Al", "Cara"), names);
    }

    @Test
    void sortReverseAlphabeticalOrdersZBeforeA() {
        List<String> names = List.of("Ann", "Zed", "Mia");

        assertEquals(List.of("Zed", "Mia", "Ann"), ComparatorSort.sortReverseAlphabetical(names));
    }
}
