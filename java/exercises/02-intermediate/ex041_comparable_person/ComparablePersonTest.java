package fewolearning.exercises.intermediate.ex041_comparable_person;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ComparablePersonTest {

    @Test
    void ordersByAgeWhenAgesDiffer() {
        ComparablePerson younger = new ComparablePerson("Bob", 20);
        ComparablePerson older = new ComparablePerson("Alice", 30);

        assertTrue(younger.compareTo(older) < 0);
        assertTrue(older.compareTo(younger) > 0);
    }

    @Test
    void ordersByNameWhenAgesAreEqual() {
        ComparablePerson alice = new ComparablePerson("Alice", 25);
        ComparablePerson bob = new ComparablePerson("Bob", 25);

        assertTrue(alice.compareTo(bob) < 0);
        assertTrue(bob.compareTo(alice) > 0);
    }

    @Test
    void isConsistentWithEqualAgeAndName() {
        ComparablePerson first = new ComparablePerson("Alice", 25);
        ComparablePerson second = new ComparablePerson("Alice", 25);

        assertEquals(0, first.compareTo(second));
    }
}
