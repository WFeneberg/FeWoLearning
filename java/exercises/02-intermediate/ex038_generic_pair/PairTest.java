package fewolearning.exercises.intermediate.ex038_generic_pair;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class PairTest {

    @Test
    void swapExchangesFirstAndSecond() {
        Pair<String, Integer> pair = new Pair<>("a", 1);

        Pair<Integer, String> swapped = pair.swap();

        assertEquals(new Pair<>(1, "a"), swapped);
    }

    @Test
    void accessorsReturnTheOriginalValues() {
        Pair<String, Integer> pair = new Pair<>("x", 7);

        assertEquals("x", pair.first());
        assertEquals(7, pair.second());
    }
}
