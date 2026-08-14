package fewolearning.exercises.advanced.ex086_money_value_object;

import java.math.BigDecimal;
import java.util.Currency;
import java.util.Objects;

/*
Exercise 086 - Money value object (reference solution).
*/
public final class Money {
    private final BigDecimal amount;
    private final Currency currency;

    public Money(BigDecimal amount, Currency currency) {
        this.amount = amount;
        this.currency = currency;
    }

    public Money add(Money other) {
        if (!currency.equals(other.currency)) {
            throw new IllegalArgumentException(
                    "Cannot add different currencies: " + currency + " and " + other.currency);
        }
        return new Money(amount.add(other.amount), currency);
    }

    @Override
    public boolean equals(Object other) {
        if (this == other) {
            return true;
        }
        if (!(other instanceof Money money)) {
            return false;
        }
        return amount.compareTo(money.amount) == 0 && currency.equals(money.currency);
    }

    @Override
    public int hashCode() {
        return Objects.hash(amount.stripTrailingZeros(), currency);
    }
}
