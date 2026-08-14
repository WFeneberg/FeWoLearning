package fewolearning.exercises.beginner.ex033_property_custom_getter

/*
Exercise 033 - Property custom getter (reference solution).
*/
class Rectangle(val width: Int, val height: Int) {
    val area: Int
        get() = width * height

    val perimeter: Int
        get() = 2 * (width + height)
}
