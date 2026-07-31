namespace FeWoLearning.Exercises.Beginner;

// Exercise 023 — VectorStruct (reference solution).
public struct VectorStruct
{
    public double X;
    public double Y;

    public VectorStruct(double x, double y)
    {
        X = x;
        Y = y;
    }

    public VectorStruct Add(VectorStruct other) => new VectorStruct(X + other.X, Y + other.Y);
}
