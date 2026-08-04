package fewolearning.exercises.beginner.ex007_safe_calls_chain

/*
Exercise 007 - Safe calls chain (beginner).

Goal:   Navigate a chain of nullable references without throwing NPEs.
Drills: ?., chaining, nullable navigation.
*/
data class Address(val city: String?)
data class Customer(val address: Address?)

fun cityOf(customer: Customer?): String? {
    TODO()
}
