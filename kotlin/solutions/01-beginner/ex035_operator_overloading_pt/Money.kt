package fewolearning.exercises.beginner.ex035_operator_overloading_pt

/*
Exercise 035 - Operator overloading (reference solution).
*/
data class Money(val cents: Long) {
    operator fun plus(other: Money): Money = Money(cents + other.cents)

    operator fun unaryMinus(): Money = Money(-cents)
}
