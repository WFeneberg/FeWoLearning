using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

public class Ex072_KeyDiffingDeepDiveTests : BunitContext
{
    private static string[] Labels(IRenderedComponent<Ex072_KeyDiffingDeepDive> cut)
        => [.. cut.FindAll(".row").Select(row => row.GetAttribute("data-label")!)];

    private static int TicksOf(IRenderedComponent<Ex072_KeyDiffingDeepDive> cut, string label)
        => cut.FindComponents<Ex072_KeyDiffingDeepDive_Row>()
            .Single(row => row.Instance.Label == label)
            .Instance.Ticks;

    [Fact]
    public void Renders_One_Row_Per_Label_In_Order()
    {
        var cut = Render<Ex072_KeyDiffingDeepDive>(p => p.Add(
            c => c.Labels, new[] { "a", "b", "c" }));

        Assert.Equal(["a", "b", "c"], Labels(cut));
    }

    // Ruling: the row's Ticks is state no parameter can restore, so where it ends up
    // after the reorder says which row instance the diff matched to which item.
    // Keyed, it follows "a"; positional, it would stay at index 0 and belong to "c".
    [Fact]
    public void Row_State_Follows_Its_Item_Across_A_Reorder()
    {
        var cut = Render<Ex072_KeyDiffingDeepDive>(p => p.Add(
            c => c.Labels, new[] { "a", "b", "c" }));
        cut.FindAll(".row")[0].QuerySelector(".tick")!.Click();
        cut.WaitForAssertion(() => Assert.Equal(1, TicksOf(cut, "a")));

        cut.Render(p => p.Add(c => c.Labels, new[] { "c", "a", "b" }));

        Assert.Equal(["c", "a", "b"], Labels(cut));
        Assert.Equal(1, TicksOf(cut, "a"));
        Assert.Equal(0, TicksOf(cut, "c"));
    }

    [Fact]
    public void Row_State_Survives_A_Removal_Of_An_Earlier_Item()
    {
        var cut = Render<Ex072_KeyDiffingDeepDive>(p => p.Add(
            c => c.Labels, new[] { "a", "b", "c" }));
        cut.FindAll(".row")[2].QuerySelector(".tick")!.Click();
        cut.WaitForAssertion(() => Assert.Equal(1, TicksOf(cut, "c")));

        cut.Render(p => p.Add(c => c.Labels, new[] { "b", "c" }));

        Assert.Equal(["b", "c"], Labels(cut));
        Assert.Equal(1, TicksOf(cut, "c"));
    }

    // The same thing said about object identity rather than about state: the
    // component instance rendering "a" before the reorder is the very same instance
    // afterwards, not a rebuilt one that happens to look alike (README §8).
    [Fact]
    public void The_Row_Instance_Itself_Moves_With_Its_Item()
    {
        var cut = Render<Ex072_KeyDiffingDeepDive>(p => p.Add(
            c => c.Labels, new[] { "a", "b", "c" }));
        var before = cut.FindComponents<Ex072_KeyDiffingDeepDive_Row>()
            .Single(row => row.Instance.Label == "a").Instance;

        cut.Render(p => p.Add(c => c.Labels, new[] { "c", "a", "b" }));

        var after = cut.FindComponents<Ex072_KeyDiffingDeepDive_Row>()
            .Single(row => row.Instance.Label == "a").Instance;
        Assert.Same(before, after);
    }

    // A new item must not inherit a neighbour's state, keyed or not.
    [Fact]
    public void An_Inserted_Item_Starts_Fresh()
    {
        var cut = Render<Ex072_KeyDiffingDeepDive>(p => p.Add(
            c => c.Labels, new[] { "a", "b" }));
        cut.FindAll(".row")[0].QuerySelector(".tick")!.Click();
        cut.WaitForAssertion(() => Assert.Equal(1, TicksOf(cut, "a")));

        cut.Render(p => p.Add(c => c.Labels, new[] { "new", "a", "b" }));

        Assert.Equal(0, TicksOf(cut, "new"));
        Assert.Equal(1, TicksOf(cut, "a"));
    }
}
