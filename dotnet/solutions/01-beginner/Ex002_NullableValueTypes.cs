namespace FeWoLearning.Exercises.Beginner;

// Exercise 002 — Nullable value types (reference solution).
public static class NullableValueTypes
{
    public static int? Add(int? a, int? b)
    {
        if (a is null || b is null)
        {
            return null;
        }

        return a.Value + b.Value;
    }
}
