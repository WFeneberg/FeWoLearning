namespace FeWoLearning.Exercises.Intermediate;

// Exercise 038 — Aggregate Totals (reference solution).
public static class AggregateTotals
{
    public static (IReadOnlyList<decimal> RunningTotals, decimal FinalTotal) ComputeRunningTotals(
        IEnumerable<decimal> prices)
    {
        var (runningTotals, finalTotal) = prices.Aggregate(
            seed: (Totals: new List<decimal>(), Sum: 0m),
            func: (acc, price) =>
            {
                var newSum = acc.Sum + price;
                acc.Totals.Add(newSum);
                return (acc.Totals, newSum);
            });

        return (runningTotals, finalTotal);
    }
}
