package fewolearning.exercises.beginner.ex021_comparator_sort;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;

/*
Exercise 021 - Comparator sort (reference solution).
*/
public final class ComparatorSort {
    private ComparatorSort() {
    }

    public static List<String> sortByLength(List<String> names) {
        List<String> sorted = new ArrayList<>(names);
        sorted.sort(Comparator.comparingInt(String::length));
        return sorted;
    }

    public static List<String> sortReverseAlphabetical(List<String> names) {
        List<String> sorted = new ArrayList<>(names);
        sorted.sort(Comparator.reverseOrder());
        return sorted;
    }
}
