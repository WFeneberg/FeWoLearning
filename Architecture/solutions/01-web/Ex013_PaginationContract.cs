namespace FeWoLearning.Architecture.Exercises.Web.Ex013;

/// <summary>
/// CreatedAt is deliberately NOT unique across the sample data. Ordering by it alone
/// gives the database licence to return ties in any order it likes, and it will use
/// that licence differently on two calls.
/// </summary>
public sealed record Item(string Id, DateTimeOffset CreatedAt);

public sealed record Page(IReadOnlyList<Item> Items, string? NextCursor);

// Exercise 013 — PaginationContract (reference solution).
public static class Ex013_PaginationContract
{
    /// <summary>
    /// CreatedAt then Id. The tie-break is not decoration: ordering by a non-unique
    /// column alone leaves the order of ties undefined, and an undefined order is
    /// allowed to differ between the request that fetched page 1 and the one that
    /// fetched page 2.
    /// </summary>
    private static IEnumerable<Item> Ordered(IReadOnlyList<Item> source) =>
        source.OrderBy(i => i.CreatedAt).ThenBy(i => i.Id, StringComparer.Ordinal);

    public static Page OffsetPage(IReadOnlyList<Item> source, int skip, int take)
    {
        var items = Ordered(source).Skip(skip).Take(take).ToList();
        return new Page(items, NextCursor: null);
    }

    public static Page CursorPage(IReadOnlyList<Item> source, string? cursor, int take)
    {
        var ordered = Ordered(source).ToList();

        // The cursor names a POSITION IN THE ORDER. Resuming after it is what makes the
        // page immune to rows appearing earlier in the sequence in the meantime.
        var start = cursor is null
            ? 0
            : ordered.FindIndex(i => i.Id == cursor) + 1;

        var items = ordered.Skip(start).Take(take).ToList();

        var next = start + items.Count < ordered.Count && items.Count > 0
            ? items[^1].Id
            : null;

        return new Page(items, next);
    }
}
