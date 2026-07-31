namespace FeWoLearning.Exercises.Intermediate;

// Exercise 037 — Group By Category (intermediate).
// Goal:   Given a list of products (each with a Category and a Price),
//         group them by category using LINQ GroupBy and, for each
//         category, compute the item count and the total price.
//         Return the results ordered alphabetically by category name.
// Drills: LINQ GroupBy, projection with Select, aggregation (Count/Sum),
//         ordering, working with records/tuples as results.
public static class GroupByCategory
{
    public static IReadOnlyList<CategorySummary> Summarize(IEnumerable<Product> products)
        => throw new NotImplementedException();
}

public sealed record Product(string Name, string Category, decimal Price);

public sealed record CategorySummary(string Category, int Count, decimal TotalPrice);
