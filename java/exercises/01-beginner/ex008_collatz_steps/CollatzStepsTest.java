package fewolearning.exercises.beginner.ex008_collatz_steps;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class CollatzStepsTest {

    @Test
    void stepsToOneCountsTheKnownSequenceForSix() {
        // 6 -> 3 -> 10 -> 5 -> 16 -> 8 -> 4 -> 2 -> 1 : 8 steps
        assertEquals(8, CollatzSteps.stepsToOne(6));
    }

    @Test
    void stepsToOneForOneIsZero() {
        assertEquals(0, CollatzSteps.stepsToOne(1));
    }

    @Test
    void stepsToOneThrowsForNonPositiveStart() {
        assertThrows(IllegalArgumentException.class, () -> CollatzSteps.stepsToOne(0));
    }

    @Test
    void isEvenIsTrueForEvenNumbers() {
        assertTrue(CollatzSteps.isEven(4L));
    }

    @Test
    void isEvenIsFalseForOddNumbers() {
        assertFalse(CollatzSteps.isEven(7L));
    }
}
