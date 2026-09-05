namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex030;

public sealed record Book(string Title, string Genre, decimal Price, int Year);

/// <summary>
/// A named, composable piece of business rule. The point is that "what counts as a
/// bargain classic" becomes an object the domain owns, instead of a Where clause copied
/// into six call sites that then drift apart.
/// </summary>
public abstract class Specification<T>
{
    public abstract bool IsSatisfiedBy(T candidate);

    public Specification<T> And(Specification<T> other) =>
        throw new NotImplementedException("TODO: Ex030 - a specification satisfied only when BOTH are");

    public Specification<T> Or(Specification<T> other) =>
        throw new NotImplementedException("TODO: Ex030 - a specification satisfied when EITHER is");

    public Specification<T> Not() =>
        throw new NotImplementedException("TODO: Ex030 - a specification satisfied exactly when this one is not");
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

// Exercise 030 — SpecificationPattern (services-data).
// Goal:   Compose business rules into new rules, and apply the composition as ONE pass
//         over the data.
// Drills: composable specifications, And/Or/Not, single evaluation.
// Passes: And / Or / Not - filter as their names promise.
//         nesting        - (A and B) or C composes further without special-casing.
//         the point      - Filter enumerates its source EXACTLY ONCE, however deeply the
//                          specification is nested.
//
// The single-enumeration fact is what makes this the specification pattern rather than a
// collection of predicates. The obvious implementation of And is
// "filter by the left, filter by the right, intersect" - it returns the right answer,
// reads perfectly well, and walks the source once per leaf. Against an in-memory list
// that is merely wasteful. Against a database it is one query per leaf plus an
// intersection in application memory, which is how a three-clause filter becomes a
// production incident.
public static class Ex030_SpecificationPattern
{
    public static IReadOnlyList<T> Filter<T>(IEnumerable<T> source, Specification<T> specification) =>
        throw new NotImplementedException(
            "TODO: Ex030 - apply the specification in a single pass over the source");
}
