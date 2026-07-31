namespace FeWoLearning.Exercises.Beginner;

// Exercise 011 — PointTuple (reference solution).
public static class PointTuple
{
    public static double Distance((double x, double y) a, (double x, double y) b)
    {
        var dx = a.x - b.x;
        var dy = a.y - b.y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
