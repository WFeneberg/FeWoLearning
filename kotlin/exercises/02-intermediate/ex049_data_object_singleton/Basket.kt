package fewolearning.exercises.intermediate.ex049_data_object_singleton

/*
Exercise 049 - Data object singleton (intermediate).

Goal:   Model an empty-state singleton using a data object with a description.
Drills: data object, singleton identity.
*/
sealed class Basket {
    data object Empty : Basket()
    data class Filled(val itemCount: Int) : Basket()
}

fun describe(basket: Basket): String {
    TODO()
}
