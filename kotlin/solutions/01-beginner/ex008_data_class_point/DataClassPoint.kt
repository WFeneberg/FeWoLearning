package fewolearning.exercises.beginner.ex008_data_class_point

/*
Exercise 008 - Data class point (reference solution).
*/
data class Point(val x: Int, val y: Int)

fun translate(point: Point, dx: Int, dy: Int): Point = point.copy(x = point.x + dx, y = point.y + dy)
