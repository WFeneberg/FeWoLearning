package fewolearning.exercises.beginner.ex035_operator_overloading_pt

/*
Exercise 035 - Operator overloading (beginner).

Goal:   Overload plus and unary minus for a Money value class.
Drills: operator functions, domain ergonomics.
*/
data class Money(val cents: Long) {
    operator fun plus(other: Money): Money {
        TODO()
    }

    operator fun unaryMinus(): Money {
        TODO()
    }
}
