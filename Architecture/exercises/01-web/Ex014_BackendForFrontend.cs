namespace FeWoLearning.Architecture.Exercises.Web.Ex014;

/// <summary>One upstream service the BFF fans out to.</summary>
public interface IUpstream
{
    string Name { get; }

    Task<string> FetchAsync(CancellationToken cancellationToken);
}

/// <summary>
/// What came back, and what did not. Errors are DATA here, not an exception: the whole
/// point of a BFF is that a page with three panels renders two of them when the third
/// service is down.
/// </summary>
public sealed record AggregateResult(
    IReadOnlyDictionary<string, string> Data,
    IReadOnlyDictionary<string, string> Errors);

// Exercise 014 — BackendForFrontend (web).
// Goal:   Fan out to several upstreams AT THE SAME TIME and return a partial answer
//         when one of them fails.
// Drills: aggregation, parallel fan-out, partial-failure semantics.
// Passes: all healthy  - Data holds one entry per upstream, keyed by Name; Errors is empty.
//         one failing  - Data holds the others, Errors holds that one keyed by Name with
//                        the exception's message. Nothing is thrown to the caller.
//         parallelism  - the facts start every upstream on a gate that only opens once
//                        ALL of them have arrived, so a sequential implementation never
//                        finishes rather than merely finishing slowly.
//         empty list   - an empty result, no exception.
//
// Testing "it was parallel" by measuring elapsed time is the tempting mistake: it makes
// the suite slow, flaky on a loaded machine, and it still passes for an implementation
// that is fast for some other reason. A rendezvous gate turns the same question into a
// deterministic one.
public static class Ex014_BackendForFrontend
{
    public static Task<AggregateResult> AggregateAsync(
        IReadOnlyList<IUpstream> upstreams,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException(
            "TODO: Ex014 - start every upstream at once, then collect successes into Data and failures into Errors");
}
