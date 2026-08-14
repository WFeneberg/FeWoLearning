package fewolearning.exercises.beginner.ex016_big_decimal_money;

import java.math.BigDecimal;
import java.math.RoundingMode;

/*
Exercise 016 - BigDecimal money (reference solution).
*/
public final class BigDecimalMoney {
    private BigDecimalMoney() {
    }

    public static BigDecimal add(BigDecimal first, BigDecimal second) {
        return first.add(second);
    }

    public static BigDecimal roundToCents(BigDecimal amount, RoundingMode roundingMode) {
        return amount.setScale(2, roundingMode);
    }
}
