package fewolearning.exercises.intermediate.ex060_junit_parameterized

import org.junit.jupiter.api.Assertions.assertFalse
import org.junit.jupiter.api.Assertions.assertTrue
import org.junit.jupiter.params.ParameterizedTest
import org.junit.jupiter.params.provider.ValueSource

class PrimeCheckerTest {

    @ParameterizedTest
    @ValueSource(ints = [2, 3, 5, 7, 11, 13, 97])
    fun isPrimeRecognizesPrimeNumbers(candidate: Int) {
        assertTrue(isPrime(candidate))
    }

    @ParameterizedTest
    @ValueSource(ints = [0, 1, 4, 6, 8, 9, 100])
    fun isPrimeRejectsNonPrimeNumbers(candidate: Int) {
        assertFalse(isPrime(candidate))
    }

    @ParameterizedTest
    @ValueSource(ints = [-5, -1])
    fun isPrimeRejectsNegativeNumbers(candidate: Int) {
        assertFalse(isPrime(candidate))
    }
}
