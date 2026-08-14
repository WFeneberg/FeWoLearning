package fewolearning.exercises.beginner.ex031_class_invariant;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class PersonTest {

    @Test
    void constructorStoresTheNameAndAge() {
        Person person = new Person("Ann", 30);

        assertEquals("Ann", person.name());
        assertEquals(30, person.age());
    }

    @Test
    void constructorRejectsANegativeAge() {
        assertThrows(IllegalArgumentException.class, () -> new Person("Ann", -1));
    }

    @Test
    void constructorRejectsAnUnrealisticallyHighAge() {
        assertThrows(IllegalArgumentException.class, () -> new Person("Ann", 151));
    }

    @Test
    void constructorRejectsABlankName() {
        assertThrows(IllegalArgumentException.class, () -> new Person("  ", 30));
    }
}
