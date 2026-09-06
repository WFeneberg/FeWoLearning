using FeWoLearning.Architecture.Exercises.Scale.Ex070;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex070_ShardingByKeyTests
{
    private static List<string> Keys(int count) =>
        [.. Enumerable.Range(0, count).Select(i => $"customer-{i}")];

    [Fact]
    public void A_Key_Always_Lands_On_The_Same_Shard()
    {
        var router = new ShardRouter(4);

        var first = router.ShardOf("customer-42");

        Assert.Equal(first, router.ShardOf("customer-42"));
        Assert.Equal(first, new ShardRouter(4).ShardOf("customer-42"));
    }

    [Fact]
    public void Every_Key_Lands_Inside_The_Cluster()
    {
        var router = new ShardRouter(4);

        Assert.All(Keys(500), k => Assert.InRange(router.ShardOf(k), 0, 3));
    }

    [Fact]
    public void Keys_Spread_Across_All_The_Shards()
    {
        // Catches a router that sends everything to shard 0 - which satisfies stability
        // and range perfectly, and is not a sharding scheme.
        var router = new ShardRouter(4);

        var used = Keys(500).Select(router.ShardOf).Distinct().OrderBy(s => s);

        Assert.Equal([0, 1, 2, 3], used);
    }

    [Fact]
    public void Rebalance_Reports_Where_Each_Moving_Key_Goes()
    {
        var router = new ShardRouter(4);
        var keys = Keys(200);

        var moves = router.Rebalance(keys, 8);

        Assert.All(moves, m =>
        {
            Assert.Equal(router.ShardOf(m.Key), m.From);
            Assert.Equal(new ShardRouter(8).ShardOf(m.Key), m.To);
            Assert.NotEqual(m.From, m.To);
        });
    }

    [Fact]
    public void Mechanism_Keys_That_Stay_Put_Are_Not_Reported()
    {
        // A plan that lists every key is safe, useless, and indistinguishable from having
        // no plan. The whole value of the report is that it is smaller than the dataset -
        // that is what makes a reshard a scheduled operation rather than a full copy.
        var router = new ShardRouter(4);
        var keys = Keys(200);
        var target = new ShardRouter(8);

        var moved = router.Rebalance(keys, 8).Select(m => m.Key).ToHashSet();

        var stayed = keys.Where(k => router.ShardOf(k) == target.ShardOf(k));

        Assert.NotEmpty(stayed);
        Assert.All(stayed, k => Assert.DoesNotContain(k, moved));
    }

    [Fact]
    public void Mechanism_Doubling_The_Cluster_Moves_Roughly_Half_The_Keys()
    {
        // The number a migration is planned around. Modulo hashing's cost is well known
        // and is not a bug to be hidden - a router that cannot report it turns a reshard
        // into an outage of unknown length. Asserted as a band, not a value: the exact
        // fraction is a property of the hash, and pinning it would be asserting the
        // implementation's arithmetic rather than the pattern's cost.
        var router = new ShardRouter(4);
        var keys = Keys(2000);

        var movedFraction = (double)router.Rebalance(keys, 8).Count / keys.Count;

        Assert.InRange(movedFraction, 0.35, 0.65);
    }

    [Fact]
    public void Adversarial_Adding_One_Shard_Moves_Far_More_Than_Doubling_Does()
    {
        // The counter-intuitive part, and the reason clusters grow by doubling. Going from
        // 4 shards to 5 moves most of the data; going from 4 to 8 moves about half. An
        // operator who does not know this schedules the cheap change and gets the
        // expensive one.
        var router = new ShardRouter(4);
        var keys = Keys(2000);

        var toFive = (double)router.Rebalance(keys, 5).Count / keys.Count;
        var toEight = (double)router.Rebalance(keys, 8).Count / keys.Count;

        Assert.True(toFive > toEight,
            $"expected 4->5 ({toFive:P0}) to move more than 4->8 ({toEight:P0})");
        Assert.InRange(toFive, 0.6, 0.95);
    }

    [Fact]
    public void Rebalancing_To_The_Same_Size_Moves_Nothing()
    {
        var router = new ShardRouter(4);

        Assert.Empty(router.Rebalance(Keys(500), 4));
    }
}
