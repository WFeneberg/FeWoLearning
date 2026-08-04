package fewolearning.exercises.beginner.ex034_record_validation;

/*
Exercise 034 - Record validation (beginner).

Goal:   Reject an invalid min/max range in the record's compact constructor.
Drills: compact constructors, invariants.
*/
public record Range(int min, int max) {
    public Range {
        throw new UnsupportedOperationException("TODO");
    }
}
