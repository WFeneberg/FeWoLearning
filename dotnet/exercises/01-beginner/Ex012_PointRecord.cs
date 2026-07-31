namespace FeWoLearning.Exercises.Beginner;

// Exercise 012 — Point Record (beginner).
// Goal:   Define a Point2D record with X and Y properties and a Translate
//         method that returns a new Point2D shifted by (dx, dy).
// Drills: record basics, value equality, immutable data.
public record Point2D(double X, double Y)
{
    public Point2D Translate(double dx, double dy) => throw new NotImplementedException();
}
