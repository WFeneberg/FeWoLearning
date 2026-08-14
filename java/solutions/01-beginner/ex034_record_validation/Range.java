package fewolearning.exercises.beginner.ex034_record_validation;

/*
Exercise 034 - Record validation (reference solution).
*/
public record Range(int min, int max) {
    public Range {
        if (min > max) {
            throw new IllegalArgumentException("min must not be greater than max: " + min + " > " + max);
        }
    }
}
