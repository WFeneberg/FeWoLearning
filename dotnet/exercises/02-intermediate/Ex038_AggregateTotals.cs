namespace FeWoLearning.Exercises.Intermediate;

// Exercise 038 — Aggregate Totals (intermediate).
// Goal:   Given a sequence of order prices, use LINQ's Aggregate overload with a
//         seed and result selector to build the running total after each order
//         alongside the final total, in a single pass over the sequence.
// Drills: LINQ Aggregate (seed + accumulator func + result selector), tuples,
//         immutable accumulation semantics.
public static class AggregateTotals
{
    // Returns the running total after each price (in input order) plus the
    // final total across all prices. An empty input yields an empty running
    // total list and a final total of 0.
    public static (IReadOnlyList<decimal> RunningTotals, decimal FinalTotal) ComputeRunningTotals(
        IEnumerable<decimal> prices) => throw new NotImplementedException();
}
