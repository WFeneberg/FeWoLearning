package fewolearning.exercises.beginner.ex011_point_record;

/*
Exercise 011 - Point record (beginner).

Goal:   Model an immutable 2D point and compute distances/translations.
Drills: records, value semantics, accessors.
*/
public record Point(double x, double y) {

    public double distanceTo(Point other) {
        throw new UnsupportedOperationException("TODO");
    }

    public Point translate(double dx, double dy) {
        throw new UnsupportedOperationException("TODO");
    }
}
