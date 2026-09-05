using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Architecture.Exercises.Web;

// Exercise 004 — MiddlewarePipeline (web).
// Goal:   Compose three middlewares into one RequestDelegate and get both halves of
//         chain-of-responsibility right: the order they run on the way in, and the
//         order they resume on the way out.
// Drills: chain of responsibility, middleware ordering, short-circuiting.
// Passes: no header - the log reads exactly
//                     ["outer:in", "gate:in", "terminal", "gate:out", "outer:out"]
//                     and the response status is 202.
//         header set - the log reads exactly
//                     ["outer:in", "gate:short-circuit", "outer:out"], the status is
//                     403, "terminal" never appears, and "outer:out" still does.
//
// The last clause is the one that matters: short-circuiting stops the pipeline going
// FORWARD, it does not abandon the middlewares already on the stack behind you. An
// implementation that returns early from the wrong place loses the outer unwind and
// silently skips whatever that middleware promised to do on the way out - logging,
// timing, disposing, committing.
public static class Ex004_MiddlewarePipeline
{
    /// <summary>The gate middleware short-circuits when the request carries this header.</summary>
    public const string ShortCircuitHeader = "X-Stop-Here";

    /// <summary>
    /// Build the pipeline. Register three middlewares in this order, each appending to
    /// <paramref name="log"/>:
    ///
    ///   outer    - appends "outer:in", calls the rest of the pipeline, appends "outer:out".
    ///   gate     - if the request has <see cref="ShortCircuitHeader"/>, appends
    ///              "gate:short-circuit", sets status 403 and does NOT call the rest of
    ///              the pipeline. Otherwise appends "gate:in", calls it, appends "gate:out".
    ///   terminal - appends "terminal" and sets status 202. Nothing runs after it.
    /// </summary>
    public static RequestDelegate Build(IServiceProvider services, IList<string> log) =>
        throw new NotImplementedException(
            "TODO: Ex004 - compose the outer, gate and terminal middlewares into one RequestDelegate");
}
