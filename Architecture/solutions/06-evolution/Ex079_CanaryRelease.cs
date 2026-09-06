namespace FeWoLearning.Architecture.Exercises.Evolution.Ex079;

public sealed record ArmStats(int Requests, int Failures)
{
    public double ErrorRatio => Requests == 0 ? 0 : (double)Failures / Requests;
}

// Exercise 079 — CanaryRelease (reference solution).
public sealed class CanaryRouter(int canaryPercent, int minSamples, double errorRatioMargin)
{
    public const string Stable = "stable";
    public const string Canary = "canary";

    private int _stableRequests, _stableFailures, _canaryRequests, _canaryFailures;

    public ArmStats StableStats => new(_stableRequests, _stableFailures);

    public ArmStats CanaryStats => new(_canaryRequests, _canaryFailures);

    public bool IsRolledBack { get; private set; }

    public string RouteFor(string requestId)
    {
        if (IsRolledBack)
            return Stable;

        // Deterministic in the request id, so a retry does not change arm - otherwise the
        // comparison is between two shuffled halves of one population rather than between
        // two populations.
        return Bucket(requestId) < canaryPercent ? Canary : Stable;
    }

    public void Record(string arm, bool succeeded)
    {
        if (arm == Canary)
        {
            _canaryRequests++;
            if (!succeeded) _canaryFailures++;
        }
        else
        {
            _stableRequests++;
            if (!succeeded) _stableFailures++;
        }

        // Patience first. A 1% canary on a quiet afternoon sees a handful of requests, and
        // two of them failing says nothing at all - a system that acts on it rolls back
        // good builds often enough that people stop believing it.
        if (IsRolledBack || _canaryRequests < minSamples)
            return;

        // RELATIVE, not absolute. "Pull it above 5% errors" fires during every upstream
        // incident, teaches everyone the canary system is noisy, and gets disabled before
        // the release it was meant to catch. The question is never "is the canary
        // failing", it is "is the canary failing MORE".
        if (CanaryStats.ErrorRatio - StableStats.ErrorRatio > errorRatioMargin)
            IsRolledBack = true;
    }

    /// <summary>An explicit stable hash - string.GetHashCode() is randomised per process.</summary>
    private static int Bucket(string requestId)
    {
        var hash = 2166136261u;

        foreach (var c in requestId)
        {
            hash ^= c;
            hash *= 16777619u;
        }

        return (int)(hash % 100);
    }
}
