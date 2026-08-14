package fewolearning.exercises.beginner.ex013_switch_expression;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class SeasonClassifierTest {

    @Test
    void seasonForMonthMapsWinterMonths() {
        assertEquals("Winter", SeasonClassifier.seasonForMonth(12));
        assertEquals("Winter", SeasonClassifier.seasonForMonth(1));
        assertEquals("Winter", SeasonClassifier.seasonForMonth(2));
    }

    @Test
    void seasonForMonthMapsSummerMonths() {
        assertEquals("Summer", SeasonClassifier.seasonForMonth(6));
        assertEquals("Summer", SeasonClassifier.seasonForMonth(7));
        assertEquals("Summer", SeasonClassifier.seasonForMonth(8));
    }

    @Test
    void seasonForMonthThrowsForAnOutOfRangeMonth() {
        assertThrows(IllegalArgumentException.class, () -> SeasonClassifier.seasonForMonth(0));
        assertThrows(IllegalArgumentException.class, () -> SeasonClassifier.seasonForMonth(13));
    }

    @Test
    void isSummerMonthIsTrueOnlyForJuneJulyAugust() {
        assertTrue(SeasonClassifier.isSummerMonth(7));
        assertFalse(SeasonClassifier.isSummerMonth(9));
    }
}
