namespace FeWoLearning.Architecture.Exercises.Scale.Ex069;

/// <summary>
/// One thing checked at startup. Required decides what happens when it is not there:
/// refuse to start, or start without it.
/// </summary>
public sealed record DependencyCheck(string Name, bool Required, Func<bool> Probe);

public sealed record StartupResult(bool Started, IReadOnlyList<string> Failed, IReadOnlyList<string> Degraded);

// Exercise 069 — StartupReadinessOrdering (scale).
// Goal:   Decide at startup which missing dependencies are worth refusing to start over,
//         and report every one of them.
// Drills: required vs optional dependencies, fail-fast, diagnosable failures.
// Passes: healthy   - Started, nothing failed, nothing degraded.
//         required  - a missing REQUIRED dependency means Started is false and its name
//                     is in Failed. Better to refuse the traffic than to accept it and
//                     fail every request.
//         optional  - a missing OPTIONAL dependency starts DEGRADED: Started is true and
//                     its name is in Degraded. A recommendation engine being down is not
//                     a reason to stop selling anything.
//         THE ONE    - EVERY check is probed, even after a required one has already
//                      failed. Returning at the first failure turns diagnosing a
//                      multi-dependency outage into one restart per dependency, each
//                      taking however long a deploy takes.
//         order     - the result does not depend on the order the checks are listed in.
//
// The required/optional split is the whole design decision, and it is one nobody makes
// by accident: the default in most codebases is that every dependency is required,
// because every dependency was added by someone who needed it. That is how a cache being
// unreachable takes the checkout down.
//
// Probing everything is the difference between a failure somebody can act on and a
// failure somebody has to bisect. "Cannot reach the database" sends an engineer to the
// database; "cannot reach the database, the broker and the identity provider" sends them
// to the network, which is where the problem actually is.
public static class Ex069_StartupReadinessOrdering
{
    public static StartupResult Start(IReadOnlyList<DependencyCheck> checks) =>
        throw new NotImplementedException(
            "TODO: Ex069 - probe every check, collect required failures and optional ones separately, and start only if nothing required is missing");
}
