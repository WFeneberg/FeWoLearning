namespace FeWoLearning.Exercises.Intermediate;

// Exercise 049 — Struct vs Class Mutation (reference solution).
public struct PointStruct
{
    public int X;
    public int Y;
}

public class PointClass
{
    public int X;
    public int Y;
}

public static class StructVsClassMutation
{
    public static void MoveStruct(PointStruct point, int dx, int dy)
    {
        // 'point' is a copy of the caller's struct (passed by value), so
        // mutating it here has no effect on the caller's original instance.
        point.X += dx;
        point.Y += dy;
    }

    public static void MoveClass(PointClass point, int dx, int dy)
    {
        // 'point' is a reference to the caller's object, so mutating its
        // fields here changes the object the caller sees too.
        point.X += dx;
        point.Y += dy;
    }
}
