namespace FeWoLearning.Exercises.Intermediate;

// Exercise 049 — Struct vs Class Mutation (intermediate).
// Goal:   Demonstrate value-type vs reference-type semantics: passing a struct
//         to a method that "mutates" it must leave the caller's copy unchanged
//         (because it is passed by value), while passing a class instance to a
//         method that mutates its field must change the caller's instance
//         (because it is passed by reference).
// Drills: struct vs class semantics, pass-by-value vs pass-by-reference,
//         mutable fields, defensive copying.
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
    // Attempts to move the struct by (dx, dy). Because PointStruct is passed
    // by value, this must NOT affect the caller's original instance.
    public static void MoveStruct(PointStruct point, int dx, int dy) => throw new NotImplementedException();

    // Moves the class instance by (dx, dy) in place. Because PointClass is a
    // reference type, this MUST affect the caller's original instance.
    public static void MoveClass(PointClass point, int dx, int dy) => throw new NotImplementedException();
}
