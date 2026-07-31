namespace FeWoLearning.Exercises.Beginner;

// Exercise 012 — Point Record (reference solution).
public record Point2D(double X, double Y)
{
    public Point2D Translate(double dx, double dy) => new(X + dx, Y + dy);
}
