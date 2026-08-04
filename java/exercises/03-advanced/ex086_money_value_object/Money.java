package fewolearning.exercises.advanced.ex086_money_value_object;

import java.math.BigDecimal;
import java.util.Currency;

/*
Exercise 086 - Money value object (advanced).

Goal:   Model money as an immutable value object with currency-safe arithmetic.
Drills: equality, precision, domain modeling.
*/
public final class Money {
    private final BigDecimal amount;
    private final Currency currency;

    public Money(BigDecimal amount, Currency currency) {
        this.amount = amount;
        this.currency = currency;
    }

    public Money add(Money other) {
        throw new UnsupportedOperationException("TODO");
    }

    @Override
    public boolean equals(Object other) {
        throw new UnsupportedOperationException("TODO");
    }

    @Override
    public int hashCode() {
        throw new UnsupportedOperationException("TODO");
    }
}
