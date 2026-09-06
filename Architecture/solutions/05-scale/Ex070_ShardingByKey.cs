namespace FeWoLearning.Architecture.Exercises.Scale.Ex070;

/// <summary>A key whose shard changed, and where it went. This is a migration plan.</summary>
public sealed record KeyMove(string Key, int From, int To);

// Exercise 070 — ShardingByKey (reference solution).
public sealed class ShardRouter(int shardCount)
{
    public int ShardCount => shardCount;

    public int ShardOf(string key) => ShardOf(key, shardCount);

    public IReadOnlyList<KeyMove> Rebalance(IReadOnlyList<string> keys, int newShardCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(newShardCount, 1);

        var moves = new List<KeyMove>();

        foreach (var key in keys)
        {
            var from = ShardOf(key, shardCount);
            var to = ShardOf(key, newShardCount);

            // Only the ones that differ. A plan that lists every key is safe, useless, and
            // indistinguishable from having no plan - the whole value of the report is
            // that it is smaller than the dataset.
            if (from != to)
                moves.Add(new KeyMove(key, from, to));
        }

        return moves;
    }

    /// <summary>
    /// An explicit FNV-style hash, not string.GetHashCode(). That one is randomised per
    /// process in .NET, so every restart would move every key - and the tests, all
    /// running in one process, would never notice.
    /// </summary>
    private static int ShardOf(string key, int count)
    {
        var hash = 2166136261u;

        foreach (var c in key)
        {
            hash ^= c;
            hash *= 16777619u;
        }

        return (int)(hash % (uint)count);
    }
}
