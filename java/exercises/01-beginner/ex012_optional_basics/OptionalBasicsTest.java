package fewolearning.exercises.beginner.ex012_optional_basics;

import org.junit.jupiter.api.Test;

import java.util.List;
import java.util.Optional;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

class OptionalBasicsTest {

    @Test
    void findFirstLongerThanReturnsTheFirstMatch() {
        List<String> values = List.of("hi", "hello", "hey", "greetings");

        Optional<String> found = OptionalBasics.findFirstLongerThan(values, 3);

        assertTrue(found.isPresent());
        assertEquals("hello", found.get());
    }

    @Test
    void findFirstLongerThanIsEmptyWhenNothingMatches() {
        List<String> values = List.of("hi", "hey");

        Optional<String> found = OptionalBasics.findFirstLongerThan(values, 10);

        assertFalse(found.isPresent());
    }

    @Test
    void describeOrDefaultReturnsThePresentValue() {
        assertEquals("hello", OptionalBasics.describeOrDefault(Optional.of("hello"), "none"));
    }

    @Test
    void describeOrDefaultReturnsTheDefaultWhenEmpty() {
        assertEquals("none", OptionalBasics.describeOrDefault(Optional.empty(), "none"));
    }
}
