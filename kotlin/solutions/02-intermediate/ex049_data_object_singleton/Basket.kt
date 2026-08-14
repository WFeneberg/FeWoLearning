package fewolearning.exercises.intermediate.ex049_data_object_singleton

sealed class Basket {
    data object Empty : Basket()
    data class Filled(val itemCount: Int) : Basket()
}

/** Describes a basket's contents for display. */
fun describe(basket: Basket): String = when (basket) {
    is Basket.Empty -> "empty"
    is Basket.Filled -> "filled with ${basket.itemCount} items"
}
