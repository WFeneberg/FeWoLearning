namespace FeWoLearning.Architecture.Exercises.Evolution.Ex078;

/// <summary>An explicit decision about one user or one tenant. Beats the percentage.</summary>
public sealed record FlagRule(string? Tenant, string? UserId, bool Enabled);

public sealed record Flag(string Name, bool DefaultValue, int RolloutPercentage, IReadOnlyList<FlagRule> Rules);

// Exercise 078 — FeatureFlagTargeting (evolution).
// Goal:   Turn a feature on for some people and not others, in a way that stays the same
//         from one request to the next.
// Drills: rule precedence, percentage rollout, stable bucketing, per-flag independence.
// Passes: precedence - a rule naming the USER wins; then a rule naming the TENANT; then
//                      the percentage; then the default.
//         0 and 100  - nobody and everybody, respectively.
//         THE FIRST   - bucketing is STABLE: the same user gets the same answer every
//                      time, and in the next process too. A flag that flickers per
//                      request is worse than no flag - the user sees the new checkout on
//                      one page and the old one on the next, and neither of them is a bug
//                      anybody can reproduce.
//         THE SECOND  - bucketing is PER FLAG: a user in the first 10% of flag A is not
//                      automatically in the first 10% of flag B. Otherwise every 10%
//                      rollout in the system lands on the same unlucky users, who
//                      experience every experiment at once and whose feedback describes
//                      a product nobody built.
//         monotonic  - raising the percentage only ADDS users. Nobody who had the feature
//                      loses it because somebody widened the rollout.
//
// The bucketing is a hash of (flag name, user id) taken modulo 100, and each of those
// three properties comes from one part of that expression: including the user makes it
// stable, including the FLAG NAME makes it independent, and comparing "bucket <
// percentage" makes it monotonic. Any of them left out looks fine in a single test and
// is felt by users.
public sealed class FlagEvaluator
{
    /// <summary>Which 0..99 bucket this user falls in for this flag.</summary>
    public int BucketOf(string flagName, string userId) =>
        throw new NotImplementedException(
            "TODO: Ex078 - hash the flag name AND the user id with an explicit stable algorithm, modulo 100");

    public bool IsEnabled(Flag flag, string tenant, string userId) =>
        throw new NotImplementedException(
            "TODO: Ex078 - a user rule, then a tenant rule, then the percentage bucket, then the default");
}
