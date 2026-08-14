package fewolearning.exercises.beginner.ex002_string_templates

/*
Exercise 002 - String templates (reference solution).
*/
fun greet(name: String, age: Int): String = "Hello, $name! You are $age years old."

fun orderSummary(item: String, quantity: Int, unitPrice: Double): String {
    val total = quantity * unitPrice
    return """
        Item: $item
        Quantity: $quantity
        Unit price: $unitPrice
        Total: $total
    """.trimIndent()
}
