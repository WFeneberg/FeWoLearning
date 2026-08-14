package fewolearning.exercises.advanced.ex072_custom_collector;

import java.util.List;
import java.util.stream.Stream;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class CustomCollectorTest {

    @Test
    void joinAllJoinsValuesWithTheSeparator() {
        assertEquals("a, b, c", CustomCollector.joinAll(List.of("a", "b", "c"), ", "));
    }

    @Test
    void joinAllOnAnEmptyListProducesAnEmptyString() {
        assertEquals("", CustomCollector.joinAll(List.of(), ", "));
    }

    @Test
    void joinAllOnASingleElementProducesThatElementWithoutASeparator() {
        assertEquals("solo", CustomCollector.joinAll(List.of("solo"), ", "));
    }

    @Test
    void joiningCanBeUsedDirectlyAsAStreamCollector() {
        String joined = Stream.of("x", "y", "z").collect(CustomCollector.joining("-"));

        assertEquals("x-y-z", joined);
    }

    @Test
    void joiningCombinesPartialResultsCorrectlyWhenRunInParallel() {
        String joined = Stream.of("1", "2", "3", "4", "5", "6", "7", "8")
                .parallel()
                .collect(CustomCollector.joining(","));

        assertEquals("1,2,3,4,5,6,7,8", joined);
    }
}
