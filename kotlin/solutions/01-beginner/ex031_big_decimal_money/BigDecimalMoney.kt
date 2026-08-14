package fewolearning.exercises.beginner.ex031_big_decimal_money

import java.math.BigDecimal
import java.math.RoundingMode

/*
Exercise 031 - BigDecimal money (reference solution).
*/
fun add(first: BigDecimal, second: BigDecimal): BigDecimal = first.add(second)

fun roundToCents(amount: BigDecimal): BigDecimal = amount.setScale(2, RoundingMode.HALF_UP)
