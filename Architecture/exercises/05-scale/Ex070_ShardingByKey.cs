namespace FeWoLearning.Architecture.Exercises.Scale.Ex070;

/// <summary>A key whose shard changed, and where it went. This is a migration plan.</summary>
public sealed record KeyMove(string Key, int From, int To);

// Exercise 070 — ShardingByKey (scale).
// Goal:   Split data across shards deterministically, and be able to say exactly what a
//         reshard would cost before doing it.
// Drills: stable hashing, shard routing, rebalancing, the price of modulo.
// Passes: stable    - the same key always lands on the same shard, in this process and
//                     the next one. NOT string.GetHashCode(), which is randomised per
//                     process in .NET: every restart would move every key.
//         in range  - ShardOf is always 0..shardCount-1.
//         spread    - many keys reach every shard.
//         Rebalance - reports exactly the keys whose shard changes, with the right From
//                     and To.
//         THE ONE    - keys that do NOT move are not reported. A plan that lists
//                     everything is safe, useless, and indistinguishable from having no
//                     plan.
//         the price  - doubling the shard count moves roughly HALF the keys, and the
//                     exercise measures it rather than hoping.
//
// Modulo hashing is the honest default and it has a well-known cost: going from N shards
// to N+1 moves almost every key, and going from N to 2N moves about half. That is not a
// bug to be hidden - it is the number a migration is planned around, and a router that
// cannot report it turns a reshard into an outage of unknown length.
//
// (Consistent hashing exists precisely to shrink that number, at the cost of a more
// complicated ring and uneven shards. This exercise makes the cost of the simple option
// measurable, which is the prerequisite for deciding whether to pay it.)
public sealed class ShardRouter(int shardCount)
{
    public int ShardCount => shardCount;

    /// <summary>Which shard <paramref name="key"/> lives on. Deterministic across processes.</summary>
    public int ShardOf(string key) =>
        throw new NotImplementedException(
            "TODO: Ex070 - hash the key with an explicit, stable algorithm and take it modulo the shard count");

    /// <summary>
    /// What would have to move if the cluster were resized to
    /// <paramref name="newShardCount"/>. Keys that stay put are not in the result.
    /// </summary>
    public IReadOnlyList<KeyMove> Rebalance(IReadOnlyList<string> keys, int newShardCount) =>
        throw new NotImplementedException(
            "TODO: Ex070 - compare each key's current shard with where it would land, and report only the ones that differ");
}
