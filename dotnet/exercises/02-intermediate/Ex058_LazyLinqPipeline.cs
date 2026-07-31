namespace FeWoLearning.Exercises.Intermediate;

// Exercise 058 — Lazy LINQ Pipeline (intermediate).
// Goal:   Build a LINQ query pipeline whose projection is deferred: it must not
//         run until the sequence is enumerated, and must run once per element
//         for every separate enumeration (LINQ does not cache results).
// Drills: deferred execution, Select projections, side effects, iterator
//         semantics (yield-based sequences re-run their source on each pull).
public static class LazyLinqPipeline
{
    // Returns a query over `source` that doubles each element, invoking
    // `onProjected` exactly once per element *per enumeration* — but only
    // once the returned sequence is actually iterated (e.g. via foreach,
    // ToList(), Count(), etc.). Building the query must not touch the source
    // or call `onProjected` at all.
    public static IEnumerable<int> BuildDoublingQuery(IEnumerable<int> source, Action<int> onProjected)
        => throw new NotImplementedException();
}
