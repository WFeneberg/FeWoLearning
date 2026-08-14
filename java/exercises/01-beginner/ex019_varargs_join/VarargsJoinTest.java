package fewolearning.exercises.beginner.ex019_varargs_join;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class VarargsJoinTest {

    @Test
    void joinCombinesPartsWithTheSeparator() {
        assertEquals("a, b, c", VarargsJoin.join(", ", "a", "b", "c"));
    }

    @Test
    void joinWithNoPartsIsAnEmptyString() {
        assertEquals("", VarargsJoin.join(", "));
    }

    @Test
    void joinWithOnePartIsThatPart() {
        assertEquals("solo", VarargsJoin.join("-", "solo"));
    }

    @Test
    void sumAllAddsUpEveryArgument() {
        assertEquals(15, VarargsJoin.sumAll(1, 2, 3, 4, 5));
    }

    @Test
    void sumAllWithNoArgumentsIsZero() {
        assertEquals(0, VarargsJoin.sumAll());
    }
}
