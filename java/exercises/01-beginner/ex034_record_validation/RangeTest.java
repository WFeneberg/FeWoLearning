package fewolearning.exercises.beginner.ex034_record_validation;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class RangeTest {

    @Test
    void constructorAcceptsAValidRangeAndExposesItsBounds() {
        Range range = new Range(1, 10);

        assertEquals(1, range.min());
        assertEquals(10, range.max());
    }

    @Test
    void constructorAcceptsAnEqualMinAndMax() {
        Range range = new Range(5, 5);

        assertEquals(5, range.min());
        assertEquals(5, range.max());
    }

    @Test
    void constructorRejectsAMinGreaterThanMax() {
        assertThrows(IllegalArgumentException.class, () -> new Range(10, 1));
    }
}
