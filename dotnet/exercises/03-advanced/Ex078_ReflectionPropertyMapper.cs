namespace FeWoLearning.Exercises.Advanced;

// Exercise 078 — Reflection-based property mapper (advanced).
// Goal:   Copy values between matching-named public properties of two POCOs
//         purely via reflection, without either type knowing about the other.
// Drills: System.Reflection (PropertyInfo, GetValue/SetValue), type
//         compatibility checks, generic constraints, member caching.
public static class ReflectionPropertyMapper
{
    // Creates a new TTarget and copies every readable source property onto a
    // writable target property of the same name whose type is assignable
    // from the source property's type. Properties with no match on the
    // target (or an incompatible type) are silently skipped.
    public static TTarget Map<TSource, TTarget>(TSource source)
        where TTarget : new()
        => throw new NotImplementedException();

    // Same mapping logic, but copies onto an existing target instance
    // instead of constructing a new one.
    public static void Map<TSource, TTarget>(TSource source, TTarget target)
        => throw new NotImplementedException();
}
