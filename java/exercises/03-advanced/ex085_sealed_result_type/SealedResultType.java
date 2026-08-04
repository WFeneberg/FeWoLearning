package fewolearning.exercises.advanced.ex085_sealed_result_type;

/*
Exercise 085 - Sealed result type (advanced).

Goal:   Model an operation outcome as either a typed success or failure.
Drills: typed success/failure outcomes.
*/
public final class SealedResultType {
    private SealedResultType() {
    }

    public sealed interface Result<T> permits Success, Failure {
    }

    public record Success<T>(T value) implements Result<T> {
    }

    public record Failure<T>(String errorMessage) implements Result<T> {
    }

    public static Result<Integer> parse(String rawValue) {
        throw new UnsupportedOperationException("TODO");
    }
}
