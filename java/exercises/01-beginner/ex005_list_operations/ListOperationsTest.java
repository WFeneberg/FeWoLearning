package fewolearning.exercises.beginner.ex005_list_operations;

import org.junit.jupiter.api.Test;

import java.util.ArrayList;
import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;

class ListOperationsTest {

    @Test
    void updateRosterAddsAndRemovesPreservingOrder() {
        List<String> roster = new ArrayList<>(List.of("Ann", "Bob", "Cara"));

        List<String> updated = ListOperations.updateRoster(roster, "Dan", "Bob");

        assertEquals(List.of("Ann", "Cara", "Dan"), updated);
    }

    @Test
    void updateRosterDoesNotMutateTheInputList() {
        List<String> roster = new ArrayList<>(List.of("Ann", "Bob"));

        ListOperations.updateRoster(roster, "Cara", "Ann");

        assertEquals(List.of("Ann", "Bob"), roster);
    }

    @Test
    void updateRosterWorksWhenTheNameToRemoveIsAbsent() {
        // List.of(...) is immutable, so this also proves updateRoster never
        // mutates its argument in place.
        List<String> roster = List.of("Ann", "Bob");

        List<String> updated = ListOperations.updateRoster(roster, "Cara", "Zed");

        assertEquals(List.of("Ann", "Bob", "Cara"), updated);
    }

    @Test
    void countNamesLongerThanCountsStrictlyLongerNames() {
        List<String> names = List.of("Al", "Bob", "Cara", "Ed");

        assertEquals(1, ListOperations.countNamesLongerThan(names, 3));
    }

    @Test
    void countNamesLongerThanCanBeZero() {
        List<String> names = List.of("Al", "Ed");

        assertEquals(0, ListOperations.countNamesLongerThan(names, 5));
    }
}
