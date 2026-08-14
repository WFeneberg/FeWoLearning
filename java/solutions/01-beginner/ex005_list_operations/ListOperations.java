package fewolearning.exercises.beginner.ex005_list_operations;

import java.util.ArrayList;
import java.util.List;

/*
Exercise 005 - List operations (reference solution).
*/
public final class ListOperations {
    private ListOperations() {
    }

    public static List<String> updateRoster(List<String> roster, String nameToAdd, String nameToRemove) {
        List<String> updated = new ArrayList<>(roster);
        updated.remove(nameToRemove);
        updated.add(nameToAdd);
        return updated;
    }

    public static int countNamesLongerThan(List<String> names, int minimumLength) {
        int count = 0;
        for (String name : names) {
            if (name.length() > minimumLength) {
                count++;
            }
        }
        return count;
    }
}
