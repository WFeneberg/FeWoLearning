package fewolearning.exercises.beginner.ex016_higher_order_basics

/*
Exercise 016 - Higher-order basics (reference solution).
*/
fun <T, R> transformAll(items: List<T>, transform: (T) -> R): List<R> = items.map(transform)
