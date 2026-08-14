package fewolearning.exercises.beginner.ex035_operator_overloading_pt

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class MoneyTest {

    @Test
    fun plusAddsTheCentsOfTwoMoneyValues() {
        assertEquals(Money(150), Money(100) + Money(50))
    }

    @Test
    fun unaryMinusNegatesTheCents() {
        assertEquals(Money(-100), -Money(100))
    }

    @Test
    fun unaryMinusIsItsOwnInverse() {
        val money = Money(250)

        assertEquals(money, -(-money))
    }
}
