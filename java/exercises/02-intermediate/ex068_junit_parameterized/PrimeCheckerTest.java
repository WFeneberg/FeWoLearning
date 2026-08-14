package fewolearning.exercises.intermediate.ex068_junit_parameterized;

import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;

import static org.junit.jupiter.api.Assertions.assertEquals;

class PrimeCheckerTest {

    @ParameterizedTest
    @CsvSource({
            "-7, false",
            "0, false",
            "1, false",
            "2, true",
            "3, true",
            "4, false",
            "17, true",
            "18, false",
            "97, true",
            "100, false"
    })
    void identifiesPrimalityCorrectly(int candidate, boolean expected) {
        assertEquals(expected, PrimeChecker.isPrime(candidate));
    }
}
