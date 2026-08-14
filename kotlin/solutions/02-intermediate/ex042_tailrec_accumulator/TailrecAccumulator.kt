package fewolearning.exercises.intermediate.ex042_tailrec_accumulator

/** Sums a list of numbers by threading an accumulator through tail recursion. */
tailrec fun sumTailrec(numbers: List<Int>, accumulator: Int = 0): Int =
    if (numbers.isEmpty()) accumulator else sumTailrec(numbers.drop(1), accumulator + numbers.first())
