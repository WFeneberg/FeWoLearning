package fewolearning.exercises.intermediate.ex060_junit_parameterized

/** Checks whether [candidate] is a prime number by trial division up to its square root. */
fun isPrime(candidate: Int): Boolean {
    if (candidate < 2) return false
    if (candidate == 2) return true
    if (candidate % 2 == 0) return false

    var divisor = 3
    while (divisor * divisor <= candidate) {
        if (candidate % divisor == 0) return false
        divisor += 2
    }
    return true
}
