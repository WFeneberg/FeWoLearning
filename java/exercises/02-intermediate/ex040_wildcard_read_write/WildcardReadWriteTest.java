package fewolearning.exercises.intermediate.ex040_wildcard_read_write;

import java.util.ArrayList;
import java.util.List;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class WildcardReadWriteTest {

    @Test
    void copyAppendsSourceElementsIntoDestination() {
        List<Integer> source = List.of(1, 2, 3);
        List<Number> destination = new ArrayList<>();

        WildcardReadWrite.copy(source, destination);

        assertEquals(List.of(1, 2, 3), destination);
    }

    @Test
    void sumAddsUpNumbersOfAnySubtype() {
        List<Double> numbers = List.of(1.5, 2.5, 3.0);

        assertEquals(7.0, WildcardReadWrite.sum(numbers), 1e-9);
    }
}
