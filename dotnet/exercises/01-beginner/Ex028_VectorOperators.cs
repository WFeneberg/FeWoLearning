namespace FeWoLearning.Exercises.Beginner;

// Exercise 028 — VectorOperators (beginner).
// Goal:   Model a simple 2D vector and overload the +, ==, and != operators
//         so vectors can be added and compared with natural syntax.
// Drills: operator overloading, value equality, struct design.
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
        => throw new NotImplementedException();

    public static bool operator ==(VectorOperators left, VectorOperators right)
        => throw new NotImplementedException();

    public static bool operator !=(VectorOperators left, VectorOperators right)
        => throw new NotImplementedException();

    public override bool Equals(object? obj) => throw new NotImplementedException();

    public override int GetHashCode() => throw new NotImplementedException();
}
