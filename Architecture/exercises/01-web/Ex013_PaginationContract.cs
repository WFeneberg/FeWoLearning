namespace FeWoLearning.Architecture.Exercises.Web.Ex013;

/// <summary>
/// CreatedAt is deliberately NOT unique across the sample data. Ordering by it alone
/// gives the database licence to return ties in any order it likes, and it will use
/// that licence differently on two calls.
/// </summary>
public sealed record Item(string Id, DateTimeOffset CreatedAt);

public sealed record Page(IReadOnlyList<Item> Items, string? NextCursor);

// Exercise 013 — PaginationContract (web).
// Goal:   Implement both pagination styles and see, on the same data, the one property
//         that separates them.
// Drills: cursor vs offset, stable ordering, page metadata.
// Passes: ordering  - both paginators order by CreatedAt ascending and break ties by
//                     Id ordinal, so the sequence is total and repeatable.
//         offset    - OffsetPage(skip, take) returns that window; NextCursor is null.
//         cursor    - CursorPage(cursor, take) returns the first `take` items strictly
//                     AFTER the cursor, and NextCursor is the Id of the last item it
//                     returned, or null on the final page.
//         the point - when a new item is inserted at the FRONT between fetching page 1
//                     and page 2, offset pagination shows one item twice and cursor
//                     pagination does not.
//
// The cursor encodes a POSITION IN THE ORDER, not a count of rows skipped. That is the
// whole difference, and it only becomes visible when the underlying data changes
// between two requests - which, in production, it always does.
public static class Ex013_PaginationContract
{
    /// <summary>Skip/take over the ordered sequence.</summary>
    public static Page OffsetPage(IReadOnlyList<Item> source, int skip, int take) =>
        throw new NotImplementedException(
            "TODO: Ex013 - order by CreatedAt then Id, then skip/take that window");

    /// <summary>
    /// The items after <paramref name="cursor"/> in the ordered sequence.
    /// A null cursor starts at the beginning.
    /// </summary>
    public static Page CursorPage(IReadOnlyList<Item> source, string? cursor, int take) =>
        throw new NotImplementedException(
            "TODO: Ex013 - order by CreatedAt then Id, resume strictly after the cursor Id, and report the next cursor");
}
