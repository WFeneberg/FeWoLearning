package fewolearning.exercises.beginner.ex017_loop_continue_break;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class LoopContinueBreakTest {

    @Test
    void sumOddNumbersUpToSumsOnlyOddValues() {
        // 1 + 3 + 5 + 7 + 9 = 25
        assertEquals(25, LoopContinueBreak.sumOddNumbersUpTo(10));
    }

    @Test
    void sumOddNumbersUpToIncludesTheLimitWhenItIsOdd() {
        // 1 + 3 + 5 = 9
        assertEquals(9, LoopContinueBreak.sumOddNumbersUpTo(5));
    }

    @Test
    void firstMultipleOfFindsTheFirstQualifyingValue() {
        assertEquals(15, LoopContinueBreak.firstMultipleOf(5, 11, 30));
    }

    @Test
    void firstMultipleOfReturnsMinusOneWhenNoneExistsInRange() {
        assertEquals(-1, LoopContinueBreak.firstMultipleOf(5, 11, 14));
    }
}
