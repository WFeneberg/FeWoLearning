package fewolearning.exercises.beginner.ex007_safe_calls_chain

/*
Exercise 007 - Safe calls chain (reference solution).
*/
data class Address(val city: String?)
data class Customer(val address: Address?)

fun cityOf(customer: Customer?): String? = customer?.address?.city
