package fewolearning.exercises.beginner.ex016_big_decimal_money;

import org.junit.jupiter.api.Test;

import java.math.BigDecimal;
import java.math.RoundingMode;

import static org.junit.jupiter.api.Assertions.assertEquals;

class BigDecimalMoneyTest {

    @Test
    void addSumsTwoAmountsExactly() {
        BigDecimal sum = BigDecimalMoney.add(new BigDecimal("1.10"), new BigDecimal("2.20"));

        assertEquals(0, new BigDecimal("3.30").compareTo(sum));
    }

    @Test
    void roundToCentsRoundsHalfUp() {
        BigDecimal rounded = BigDecimalMoney.roundToCents(new BigDecimal("1.005"), RoundingMode.HALF_UP);

        assertEquals(0, new BigDecimal("1.01").compareTo(rounded));
        assertEquals(2, rounded.scale());
    }

    @Test
    void roundToCentsRoundsDownWhenAskedTo() {
        BigDecimal rounded = BigDecimalMoney.roundToCents(new BigDecimal("1.999"), RoundingMode.DOWN);

        assertEquals(0, new BigDecimal("1.99").compareTo(rounded));
        assertEquals(2, rounded.scale());
    }
}
