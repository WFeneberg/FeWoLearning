namespace FeWoLearning.Architecture.Exercises.Web.Ex012;

/// <summary>The one internal model. Both versions are views onto it.</summary>
public sealed record Order(string Id, decimal Subtotal, decimal Tax)
{
    public decimal Total => Subtotal + Tax;
}

/// <summary>The contract v1 clients parse. It must never grow a required field.</summary>
public sealed record OrderV1(string Id, decimal Total);

/// <summary>v2 splits the total out. v1 clients are not told and do not care.</summary>
public sealed record OrderV2(string Id, decimal Subtotal, decimal Tax, decimal Total);

// Exercise 012 — ApiVersioning (web).
// Goal:   Serve two contract versions from one internal model, so a v1 client written
//         before v2 existed keeps working unchanged.
// Drills: versioned contracts, v1/v2 coexistence, additive vs breaking change.
// Passes: Render(order, 1) - an OrderV1 carrying only Id and Total.
//         Render(order, 2) - an OrderV2 carrying Subtotal, Tax and Total.
//         both             - report the SAME Total for the same order, because they are
//                            two views of one model rather than two models.
//         Render(order, 3) - throws NotSupportedException whose message names the
//                            version that was asked for.
//
// Versioning goes wrong in a way that looks like progress: v2 needs the tax broken
// out, so the fields get added to the existing contract "additively", every client is
// told the new fields are optional, and the v1 client that validates its response
// against a closed schema starts rejecting everything. Two contract types, one model.
public static class Ex012_ApiVersioning
{
    /// <summary>
    /// Project the order onto the contract for <paramref name="version"/>. Return type
    /// is object because the two versions are genuinely different types - that is the
    /// point.
    /// </summary>
    public static object Render(Order order, int version) =>
        throw new NotImplementedException(
            "TODO: Ex012 - project the order onto OrderV1 or OrderV2, and reject any other version by name");
}
