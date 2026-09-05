namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex030;

public sealed record Book(string Title, string Genre, decimal Price, int Year);

// Exercise 030 — SpecificationPattern (reference solution).
public abstract class Specification<T>
{
    public abstract bool IsSatisfiedBy(T candidate);

    // Each combinator returns a SPECIFICATION, not a filtered collection. That is what
    // keeps the composition a single predicate: however deep the tree gets, evaluating
    // it is still one call per candidate.
    public Specification<T> And(Specification<T> other) => new AndSpecification<T>(this, other);

    public Specification<T> Or(Specification<T> other) => new OrSpecification<T>(this, other);

    public Specification<T> Not() => new NotSpecification<T>(this);
}

internal sealed class AndSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
{
    public override bool IsSatisfiedBy(T candidate) =>
        left.IsSatisfiedBy(candidate) && right.IsSatisfiedBy(candidate);
}

internal sealed class OrSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
{
    public override bool IsSatisfiedBy(T candidate) =>
        left.IsSatisfiedBy(candidate) || right.IsSatisfiedBy(candidate);
}

internal sealed class NotSpecification<T>(Specification<T> inner) : Specification<T>
{
    public override bool IsSatisfiedBy(T candidate) => !inner.IsSatisfiedBy(candidate);
}

public sealed class GenreIs(string genre) : Specification<Book>
{
    public override bool IsSatisfiedBy(Book candidate) =>
        string.Equals(candidate.Genre, genre, StringComparison.OrdinalIgnoreCase);
}

public sealed class CheaperThan(decimal price) : Specification<Book>
{
    public override bool IsSatisfiedBy(Book candidate) => candidate.Price < price;
}

public sealed class PublishedBefore(int year) : Specification<Book>
{
    public override bool IsSatisfiedBy(Book candidate) => candidate.Year < year;
}

public static class Ex030_SpecificationPattern
{
    public static IReadOnlyList<T> Filter<T>(IEnumerable<T> source, Specification<T> specification) =>
        // One Where, one pass. "Filter by the left, filter by the right, intersect"
        // returns the same answer and walks the source once per leaf - against a
        // database, one query per leaf plus an intersection in application memory.
        [.. source.Where(specification.IsSatisfiedBy)];
}
