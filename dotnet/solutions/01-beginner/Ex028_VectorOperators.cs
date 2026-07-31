namespace FeWoLearning.Exercises.Beginner;

// Exercise 028 — VectorOperators (reference solution).
public readonly struct VectorOperators
{
    public int X { get; }
    public int Y { get; }

    public VectorOperators(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static VectorOperators operator +(VectorOperators left, VectorOperators right)
        => new(left.X + right.X, left.Y + right.Y);

    public static bool operator ==(VectorOperators left, VectorOperators right)
        => left.X == right.X && left.Y == right.Y;

    public static bool operator !=(VectorOperators left, VectorOperators right)
        => !(left == right);

    public override bool Equals(object? obj)
        => obj is VectorOperators other && this == other;

    public override int GetHashCode() => HashCode.Combine(X, Y);
}
