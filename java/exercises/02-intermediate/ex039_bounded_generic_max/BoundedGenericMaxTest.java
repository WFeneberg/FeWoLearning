package fewolearning.exercises.intermediate.ex039_bounded_generic_max;

import java.util.List;
import java.util.NoSuchElementException;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class BoundedGenericMaxTest {

    @Test
    void findsTheMaxOfIntegers() {
        assertEquals(9, BoundedGenericMax.max(List.of(3, 9, 1, 7)));
    }

    @Test
    void findsTheMaxOfStrings() {
        assertEquals("pear", BoundedGenericMax.max(List.of("apple", "pear", "kiwi")));
    }

    @Test
    void throwsOnAnEmptyList() {
        assertThrows(NoSuchElementException.class, () -> BoundedGenericMax.max(List.of()));
    }
}
