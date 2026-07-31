namespace FeWoLearning.Exercises.Beginner;

// Exercise 023 — VectorStruct (beginner).
// Goal:   Implement a 2D vector as a struct (value type) with an Add method
//         that returns a new vector. Because structs are copied by value,
//         mutating a copy must never affect the original instance.
// Drills: struct value semantics, copy-by-value, immutable-style operations.
public struct VectorStruct
{
    public double X;
    public double Y;

    public VectorStruct(double x, double y)
    {
        X = x;
        Y = y;
    }

    public VectorStruct Add(VectorStruct other) => throw new NotImplementedException();
}
