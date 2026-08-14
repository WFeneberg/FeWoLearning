package fewolearning.exercises.intermediate.ex048_collector_teeing;

import java.util.List;
import java.util.NoSuchElementException;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class CollectorTeeingTest {

    @Test
    void minAndMaxComputesBothBoundsInOnePass() {
        CollectorTeeing.MinMax result = CollectorTeeing.minAndMax(List.of(5, 1, 9, 3, 7));

        assertEquals(new CollectorTeeing.MinMax(1, 9), result);
    }

    @Test
    void minAndMaxWorksForASingleElementList() {
        CollectorTeeing.MinMax result = CollectorTeeing.minAndMax(List.of(4));

        assertEquals(new CollectorTeeing.MinMax(4, 4), result);
    }

    @Test
    void minAndMaxThrowsForAnEmptyList() {
        assertThrows(NoSuchElementException.class, () -> CollectorTeeing.minAndMax(List.of()));
    }
}
