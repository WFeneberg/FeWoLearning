namespace FeWoLearning.Exercises.Intermediate;

// Exercise 058 — Lazy LINQ Pipeline (reference solution).
public static class LazyLinqPipeline
{
    public static IEnumerable<int> BuildDoublingQuery(IEnumerable<int> source, Action<int> onProjected)
        => source.Select(x =>
        {
            onProjected(x);
            return x * 2;
        });
}
