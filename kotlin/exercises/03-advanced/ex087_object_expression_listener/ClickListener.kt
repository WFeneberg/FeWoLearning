package fewolearning.exercises.advanced.ex087_object_expression_listener

/*
Exercise 087 - Object expression listener (advanced).

Goal:   Implement a click listener inline using an anonymous object expression.
Drills: anonymous objects, interfaces.
*/
interface ClickListener {
    fun onClick(x: Int, y: Int)
}

fun loggingListener(log: MutableList<String>): ClickListener {
    TODO()
}
