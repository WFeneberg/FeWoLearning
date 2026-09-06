namespace FeWoLearning.Architecture.Exercises.Evolution.Ex078;

/// <summary>An explicit decision about one user or one tenant. Beats the percentage.</summary>
public sealed record FlagRule(string? Tenant, string? UserId, bool Enabled);

public sealed record Flag(string Name, bool DefaultValue, int RolloutPercentage, IReadOnlyList<FlagRule> Rules);

// Exercise 078 — FeatureFlagTargeting (reference solution).
public sealed class FlagEvaluator
{
    public int BucketOf(string flagName, string userId)
    {
        // The FLAG NAME is in the hash, and that is not decoration: without it every 10%
        // rollout in the system lands on the same unlucky users, who experience every
        // experiment at once and whose feedback describes a product nobody built.
        //
        // An explicit FNV hash, not string.GetHashCode(), which is randomised per process
        // in .NET - so a user would move bucket on every restart, and the flag would
        // flicker across deployments rather than within them.
        var hash = 2166136261u;

        foreach (var c in flagName + ":" + userId)
        {
            hash ^= c;
            hash *= 16777619u;
        }

        return (int)(hash % 100);
    }

    public bool IsEnabled(Flag flag, string tenant, string userId)
    {
        // Most specific first. A user rule exists precisely to override whatever the
        // tenant or the rollout would have said - it is how support turns something off
        // for one complaining customer without touching anybody else.
        if (flag.Rules.FirstOrDefault(r => r.UserId == userId) is { } userRule)
            return userRule.Enabled;

        if (flag.Rules.FirstOrDefault(r => r.UserId is null && r.Tenant == tenant) is { } tenantRule)
            return tenantRule.Enabled;

        // "bucket < percentage", not "bucket == something" or a re-hash per evaluation.
        // This comparison is what makes the rollout monotonic: raising the percentage can
        // only add users, so nobody loses the feature because somebody widened it.
        if (flag.RolloutPercentage > 0)
            return BucketOf(flag.Name, userId) < flag.RolloutPercentage;

        return flag.DefaultValue;
    }
}
