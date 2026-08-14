package fewolearning.exercises.beginner.ex004_array_statistics;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class ArrayStatisticsTest {

    @Test
    void minFindsTheSmallestValue() {
        assertEquals(-3, ArrayStatistics.min(new int[] {5, -3, 10, 0}));
    }

    @Test
    void maxFindsTheLargestValue() {
        assertEquals(10, ArrayStatistics.max(new int[] {5, -3, 10, 0}));
    }

    @Test
    void averageComputesTheMean() {
        assertEquals(3.0, ArrayStatistics.average(new int[] {1, 2, 3, 4, 5}), 1e-9);
    }

    @Test
    void singleElementArray() {
        assertEquals(7, ArrayStatistics.min(new int[] {7}));
        assertEquals(7, ArrayStatistics.max(new int[] {7}));
        assertEquals(7.0, ArrayStatistics.average(new int[] {7}), 1e-9);
    }

    @Test
    void emptyArrayIsRejected() {
        assertThrows(IllegalArgumentException.class, () -> ArrayStatistics.min(new int[] {}));
        assertThrows(IllegalArgumentException.class, () -> ArrayStatistics.max(new int[] {}));
        assertThrows(IllegalArgumentException.class, () -> ArrayStatistics.average(new int[] {}));
    }
}
