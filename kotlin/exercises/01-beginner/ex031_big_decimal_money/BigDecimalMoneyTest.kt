package fewolearning.exercises.beginner.ex031_big_decimal_money

import java.math.BigDecimal
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class BigDecimalMoneyTest {

    @Test
    fun addSumsTwoMonetaryAmountsExactly() {
        val result = add(BigDecimal("1.50"), BigDecimal("2.30"))

        assertEquals(0, result.compareTo(BigDecimal("3.80")))
    }

    @Test
    fun roundToCentsRoundsHalfUpToTwoDecimalPlaces() {
        assertEquals(0, roundToCents(BigDecimal("2.345")).compareTo(BigDecimal("2.35")))
        assertEquals(0, roundToCents(BigDecimal("2.344")).compareTo(BigDecimal("2.34")))
    }

    @Test
    fun roundToCentsPadsAWholeAmountToTwoDecimalPlaces() {
        val result = roundToCents(BigDecimal("5"))

        assertEquals(0, result.compareTo(BigDecimal("5.00")))
        assertEquals(2, result.scale())
    }
}
