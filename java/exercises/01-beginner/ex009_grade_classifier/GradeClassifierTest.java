package fewolearning.exercises.beginner.ex009_grade_classifier;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class GradeClassifierTest {

    @Test
    void classifyReturnsAForNinetyAndAbove() {
        assertEquals("A", GradeClassifier.classify(95));
        assertEquals("A", GradeClassifier.classify(90));
    }

    @Test
    void classifyReturnsFBelowSixty() {
        assertEquals("F", GradeClassifier.classify(59));
        assertEquals("F", GradeClassifier.classify(0));
    }

    @Test
    void classifyHandlesTheMiddleRanges() {
        assertEquals("B", GradeClassifier.classify(85));
        assertEquals("C", GradeClassifier.classify(75));
        assertEquals("D", GradeClassifier.classify(65));
    }

    @Test
    void classifyThrowsForScoresOutsideZeroToOneHundred() {
        assertThrows(IllegalArgumentException.class, () -> GradeClassifier.classify(-1));
        assertThrows(IllegalArgumentException.class, () -> GradeClassifier.classify(101));
    }

    @Test
    void isPassingIsTrueAtSixtyAndAbove() {
        assertTrue(GradeClassifier.isPassing(60));
        assertFalse(GradeClassifier.isPassing(59));
    }
}
