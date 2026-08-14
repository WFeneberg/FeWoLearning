package fewolearning.exercises.advanced.ex086_money_value_object;

import java.math.BigDecimal;
import java.util.Currency;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class MoneyTest {

    private static final Currency USD = Currency.getInstance("USD");
    private static final Currency EUR = Currency.getInstance("EUR");

    @Test
    void addingTheSameCurrencySumsTheAmounts() {
        Money ten = new Money(new BigDecimal("10.00"), USD);
        Money five = new Money(new BigDecimal("5.00"), USD);

        assertEquals(new Money(new BigDecimal("15.00"), USD), ten.add(five));
    }

    @Test
    void addingDifferentCurrenciesThrows() {
        Money ten = new Money(new BigDecimal("10.00"), USD);
        Money fiveEuros = new Money(new BigDecimal("5.00"), EUR);

        assertThrows(IllegalArgumentException.class, () -> ten.add(fiveEuros));
    }

    @Test
    void amountsWithDifferentScalesButTheSameValueAreEqualAndHashConsistently() {
        Money scaledLow = new Money(new BigDecimal("1.5"), USD);
        Money scaledHigh = new Money(new BigDecimal("1.50"), USD);

        assertEquals(scaledLow, scaledHigh);
        assertEquals(scaledLow.hashCode(), scaledHigh.hashCode());
    }

    @Test
    void sameAmountInDifferentCurrenciesAreNotEqual() {
        Money tenUsd = new Money(new BigDecimal("10.00"), USD);
        Money tenEur = new Money(new BigDecimal("10.00"), EUR);

        assertNotEquals(tenUsd, tenEur);
    }

    @Test
    void isNotEqualToAnUnrelatedObject() {
        Money tenUsd = new Money(new BigDecimal("10.00"), USD);

        assertFalse(tenUsd.equals("10.00 USD"));
    }
}
