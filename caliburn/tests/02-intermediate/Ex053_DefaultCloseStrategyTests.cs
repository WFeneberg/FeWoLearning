using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex053_DefaultCloseStrategyTests : CaliburnCoreContext
{
    [Fact]
    public async Task All_Willing_With_The_Default_Flag_CloseCanOccur_Is_True_And_Children_Is_Everyone()
    {
        var w1 = new Ex053_Item();
        var w2 = new Ex053_Item();

        var result = await Ex053_DefaultCloseStrategy.RunAsync(new[] { w1, w2 }, closeConductedItemsWhenConductorCannotClose: false);

        Assert.True(result.CloseCanOccur);
        Assert.Equal(new[] { w1, w2 }, result.Children);
    }

    [Fact]
    public async Task One_Refusing_Item_With_The_Default_Flag_False_CloseCanOccur_Is_False_And_Children_Is_Empty()
    {
        var w = new Ex053_Item();
        var r = new Ex053_Item { RefuseClose = true };

        var result = await Ex053_DefaultCloseStrategy.RunAsync(new[] { w, r }, closeConductedItemsWhenConductorCannotClose: false);

        Assert.False(result.CloseCanOccur);
        // The sharp lesson: even though w was willing, NOTHING comes back under the default flag.
        Assert.Empty(result.Children);
    }

    [Fact]
    public async Task One_Refusing_Item_With_The_Flag_True_CloseCanOccur_Is_Still_False_But_Children_Holds_The_Willing_One()
    {
        var w = new Ex053_Item();
        var r = new Ex053_Item { RefuseClose = true };

        var result = await Ex053_DefaultCloseStrategy.RunAsync(new[] { w, r }, closeConductedItemsWhenConductorCannotClose: true);

        // The flag does NOT change whether the group may close - a stub that flips
        // CloseCanOccur to true whenever the flag is true fails right here.
        Assert.False(result.CloseCanOccur);
        Assert.Equal(new[] { w }, result.Children);
    }

    [Fact]
    public async Task All_Willing_With_The_Flag_True_Behaves_The_Same_As_The_Default()
    {
        var w1 = new Ex053_Item();
        var w2 = new Ex053_Item();

        var result = await Ex053_DefaultCloseStrategy.RunAsync(new[] { w1, w2 }, closeConductedItemsWhenConductorCannotClose: true);

        Assert.True(result.CloseCanOccur);
        Assert.Equal(new[] { w1, w2 }, result.Children);
    }

    [Fact]
    public async Task All_Refusing_Produces_An_Empty_Children_List_Regardless_Of_The_Flag()
    {
        var r1 = new Ex053_Item { RefuseClose = true };
        var r2 = new Ex053_Item { RefuseClose = true };

        var withFlagTrue = await Ex053_DefaultCloseStrategy.RunAsync(new[] { r1, r2 }, closeConductedItemsWhenConductorCannotClose: true);

        // A stub that treats "flag is true" as "return everyone regardless of willingness"
        // fails here: with nobody willing, Children must still be empty.
        Assert.False(withFlagTrue.CloseCanOccur);
        Assert.Empty(withFlagTrue.Children);

        var withFlagFalse = await Ex053_DefaultCloseStrategy.RunAsync(new[] { r1, r2 }, closeConductedItemsWhenConductorCannotClose: false);
        Assert.False(withFlagFalse.CloseCanOccur);
        Assert.Empty(withFlagFalse.Children);
    }
}
