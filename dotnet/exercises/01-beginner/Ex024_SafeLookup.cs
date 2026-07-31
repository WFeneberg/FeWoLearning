namespace FeWoLearning.Exercises.Beginner;

// Exercise 024 — SafeLookup (beginner).
// Goal:   Implement TryFind, which searches a string array for the first
//         element matching a predicate and returns it, or null if none match.
//         Use nullable reference type annotations to make the "no result"
//         case explicit in the method signature.
// Drills: nullable reference types, arrays, predicates (Func<string, bool>).
public static class SafeLookup
{
    public static string? TryFind(string[] items, Func<string, bool> predicate) => throw new NotImplementedException();
}
