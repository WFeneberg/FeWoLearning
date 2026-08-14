package fewolearning.exercises.beginner.ex027_sequence_lazy_basics

/*
Exercise 027 - Sequence lazy basics (reference solution).
*/
fun firstSquareAbove(numbers: List<Int>, threshold: Int): Int? =
    numbers.asSequence()
        .map { it * it }
        .firstOrNull { it > threshold }
