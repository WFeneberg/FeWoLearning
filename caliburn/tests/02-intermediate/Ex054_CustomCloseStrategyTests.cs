using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex054_CustomCloseStrategyTests : CaliburnCoreContext
{
    [Fact]
    public async Task Majority_Willing_Two_Of_Three_Closes_Even_Though_One_Refuses()
    {
        var w1 = new Ex054_Item();
        var w2 = new Ex054_Item();
        var r = new Ex054_Item { RefuseClose = true };

        var result = await new Ex054_MajorityRulesCloseStrategy().ExecuteAsync(new[] { w1, w2, r }, CancellationToken.None);

        Assert.True(result.CloseCanOccur);
        // Children is the willing subset only - the refuser is not in it, even though the
        // majority carried the vote.
        Assert.Equal(new[] { w1, w2 }, result.Children);
    }

    [Fact]
    public async Task Majority_Refusing_Two_Of_Three_Blocks_The_Close_But_Still_Surfaces_The_One_Willing_Item()
    {
        var w = new Ex054_Item();
        var r1 = new Ex054_Item { RefuseClose = true };
        var r2 = new Ex054_Item { RefuseClose = true };

        var result = await new Ex054_MajorityRulesCloseStrategy().ExecuteAsync(new[] { w, r1, r2 }, CancellationToken.None);

        // A stub that answers true whenever ANY item is willing (an "OR" instead of a real vote
        // count) fails right here.
        Assert.False(result.CloseCanOccur);
        // Unlike Caliburn's own DefaultCloseStrategy with its flag left false (ex053), this
        // strategy always reports the willing subset regardless of the overall outcome.
        Assert.Equal(new[] { w }, result.Children);
    }

    [Fact]
    public async Task Exactly_Half_Willing_Does_Not_Reach_A_Majority()
    {
        var w1 = new Ex054_Item();
        var w2 = new Ex054_Item();
        var r1 = new Ex054_Item { RefuseClose = true };
        var r2 = new Ex054_Item { RefuseClose = true };

        var result = await new Ex054_MajorityRulesCloseStrategy().ExecuteAsync(new[] { w1, w2, r1, r2 }, CancellationToken.None);

        // A stub using ">=" instead of a strict ">" fails here: exactly half is not a majority.
        Assert.False(result.CloseCanOccur);
        Assert.Equal(new[] { w1, w2 }, result.Children);
    }

    [Fact]
    public async Task All_Willing_CloseCanOccur_Is_True_And_Children_Is_Everyone()
    {
        var w1 = new Ex054_Item();
        var w2 = new Ex054_Item();

        var result = await new Ex054_MajorityRulesCloseStrategy().ExecuteAsync(new[] { w1, w2 }, CancellationToken.None);

        Assert.True(result.CloseCanOccur);
        Assert.Equal(new[] { w1, w2 }, result.Children);
    }

    [Fact]
    public async Task Assigning_This_Strategy_To_A_Real_Conductors_CloseStrategy_Overrides_Caliburns_Own_Default()
    {
        var conductor = new Conductor<Ex054_Item>.Collection.AllActive();
        var w1 = new Ex054_Item();
        var w2 = new Ex054_Item();
        var r = new Ex054_Item { RefuseClose = true };
        await conductor.ActivateItemAsync(w1);
        await conductor.ActivateItemAsync(w2);
        await conductor.ActivateItemAsync(r);

        // With Caliburn's own DefaultCloseStrategy (never assigned here, but the built-in
        // default every ConductorBase<T> starts with), one refusing item among three would make
        // CanCloseAsync false. Plugging in the majority-vote strategy instead must change that
        // outcome - proving CloseStrategy is genuinely honoured, not just accepted and ignored.
        conductor.CloseStrategy = new Ex054_MajorityRulesCloseStrategy();

        var canClose = await conductor.CanCloseAsync();

        Assert.True(canClose);
    }
}
