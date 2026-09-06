namespace FeWoLearning.Architecture.Exercises.Evolution.Ex079;

public sealed record ArmStats(int Requests, int Failures)
{
    public double ErrorRatio => Requests == 0 ? 0 : (double)Failures / Requests;
}

// Exercise 079 — CanaryRelease (evolution).
// Goal:   Send a slice of real traffic to a new build, judge it against the old one, and
//         pull it if it is worse.
// Drills: fractional routing, stable assignment, relative comparison, statistical patience.
// Passes: split     - roughly canaryPercent of requests go to the canary.
//         stable    - the same request id always routes the same way, so a retry does not
//                     change arm and a comparison is between two populations rather than
//                     one shuffled one.
//         rollback  - the canary is pulled when its error ratio exceeds the stable arm's
//                     by more than the margin.
//         THE FIRST  - below minSamples, NOTHING is pulled. Two failures out of two is not
//                     evidence; on a small canary it is a Tuesday.
//         THE SECOND - when BOTH arms are failing equally, the canary is NOT pulled. The
//                     dependency is down, the new build is innocent, and rolling back
//                     changes nothing while hiding the actual cause.
//         after      - once rolled back, ALL traffic goes to stable.
//
// The relative comparison is what makes this a canary rather than a health check. An
// absolute threshold - "pull it above 5% errors" - fires during every upstream incident,
// teaches everyone that the canary system is noisy, and gets disabled before the release
// it was meant to catch. The question is never "is the canary failing", it is "is the
// canary failing MORE".
//
// And minSamples is the difference between a canary and a superstition. A 1% canary on a
// quiet afternoon sees a handful of requests; two of them failing says nothing at all,
// and a system that acts on it will roll back good builds often enough that people stop
// believing it.
public sealed class CanaryRouter(int canaryPercent, int minSamples, double errorRatioMargin)
{
    public const string Stable = "stable";
    public const string Canary = "canary";

    public ArmStats StableStats =>
        throw new NotImplementedException("TODO: Ex079 - requests and failures on the stable arm");

    public ArmStats CanaryStats =>
        throw new NotImplementedException("TODO: Ex079 - requests and failures on the canary arm");

    public bool IsRolledBack =>
        throw new NotImplementedException("TODO: Ex079 - has the canary been pulled");

    /// <summary>Which arm serves this request. Deterministic in the request id.</summary>
    public string RouteFor(string requestId) =>
        throw new NotImplementedException(
            "TODO: Ex079 - send everything to stable once rolled back, otherwise bucket the request id by canaryPercent");

    /// <summary>Record an outcome, and pull the canary if the evidence now says to.</summary>
    public void Record(string arm, bool succeeded) =>
        throw new NotImplementedException(
            "TODO: Ex079 - count it, then roll back only once the canary has enough samples AND is failing more than stable by the margin");
}
