package fewolearning.exercises.beginner.ex007_sum_of_digits;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class SumOfDigitsTest {

    @Test
    void sumDigitsAddsUpEachDigit() {
        assertEquals(6, SumOfDigits.sumDigits(123));
    }

    @Test
    void sumDigitsOfASingleDigitIsItself() {
        assertEquals(7, SumOfDigits.sumDigits(7));
    }

    @Test
    void sumDigitsUsesTheAbsoluteValueForNegativeNumbers() {
        assertEquals(6, SumOfDigits.sumDigits(-123));
    }

    @Test
    void digitalRootReducesUntilASingleDigitRemains() {
        // 9875 -> 9+8+7+5 = 29 -> 2+9 = 11 -> 1+1 = 2
        assertEquals(2, SumOfDigits.digitalRoot(9875));
    }

    @Test
    void digitalRootOfZeroIsZero() {
        assertEquals(0, SumOfDigits.digitalRoot(0));
    }
}
