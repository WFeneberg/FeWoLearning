package fewolearning.exercises.intermediate.ex054_pair_triple_transform

/** Computes the min, max, and average of a list as a Triple. */
fun minMaxAverage(numbers: List<Int>): Triple<Int, Int, Double> =
    Triple(numbers.min(), numbers.max(), numbers.average())
