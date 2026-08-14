package fewolearning.exercises.advanced.ex085_sealed_result_type;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertInstanceOf;
import static org.junit.jupiter.api.Assertions.assertTrue;

class SealedResultTypeTest {

    @Test
    void parsingAValidIntegerYieldsASuccessWithItsValue() {
        SealedResultType.Result<Integer> result = SealedResultType.parse("42");

        SealedResultType.Success<Integer> success = assertInstanceOf(SealedResultType.Success.class, result);
        assertEquals(42, success.value());
    }

    @Test
    void parsingAnInvalidIntegerYieldsAFailureWithAMessage() {
        SealedResultType.Result<Integer> result = SealedResultType.parse("not-a-number");

        SealedResultType.Failure<Integer> failure = assertInstanceOf(SealedResultType.Failure.class, result);
        assertTrue(failure.errorMessage().contains("not-a-number"));
    }

    @Test
    void exhaustiveSwitchOverTheSealedInterfaceHandlesBothVariants() {
        SealedResultType.Result<Integer> success = SealedResultType.parse("7");
        SealedResultType.Result<Integer> failure = SealedResultType.parse("nope");

        assertEquals("ok:7", describe(success));
        assertEquals("error", describe(failure).substring(0, 5));
    }

    private static String describe(SealedResultType.Result<Integer> result) {
        return switch (result) {
            case SealedResultType.Success<Integer> s -> "ok:" + s.value();
            case SealedResultType.Failure<Integer> f -> "error:" + f.errorMessage();
        };
    }
}
