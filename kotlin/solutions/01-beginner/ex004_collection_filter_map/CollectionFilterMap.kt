package fewolearning.exercises.beginner.ex004_collection_filter_map

/*
Exercise 004 - Collection filter/map (reference solution).
*/
fun evenSquares(numbers: List<Int>): List<Int> = numbers.filter { it % 2 == 0 }.map { it * it }
