package fewolearning.exercises.intermediate.ex068_junit_parameterized;

/*
Exercise 068 - JUnit parameterized (reference solution).
*/
public final class PrimeChecker {
    private PrimeChecker() {
    }

    public static boolean isPrime(int candidate) {
        if (candidate < 2) {
            return false;
        }
        for (int divisor = 2; (long) divisor * divisor <= candidate; divisor++) {
            if (candidate % divisor == 0) {
                return false;
            }
        }
        return true;
    }
}
