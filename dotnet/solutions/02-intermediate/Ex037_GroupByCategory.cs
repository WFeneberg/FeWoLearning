namespace FeWoLearning.Exercises.Intermediate;

// Exercise 037 — Group By Category (reference solution).
public static class GroupByCategory
{
    public static IReadOnlyList<CategorySummary> Summarize(IEnumerable<Product> products)
        => products
            .GroupBy(p => p.Category)
            .Select(g => new CategorySummary(g.Key, g.Count(), g.Sum(p => p.Price)))
            .OrderBy(s => s.Category, StringComparer.Ordinal)
            .ToList();
}

public sealed record Product(string Name, string Category, decimal Price);

public sealed record CategorySummary(string Category, int Count, decimal TotalPrice);
