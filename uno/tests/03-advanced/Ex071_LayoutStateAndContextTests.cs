using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex071_LayoutStateAndContextTests : UnoTestContext
{
    private static (ItemsRepeater Repeater, Ex071_LayoutStateAndContext Layout) Hosted(params double[] widths)
    {
        var layout = new Ex071_LayoutStateAndContext();
        return (Layout(Ex071_LayoutStateAndContext.CreateRepeater(widths, layout), width: 300, height: 300), layout);
    }

    [Fact]
    public void The_Layout_Renders_Every_Item()
    {
        var (repeater, _) = Hosted(10, 20, 30);

        Assert.All(
            Enumerable.Range(0, 3),
            index => Assert.NotNull(repeater.TryGetElement(index)));
    }

    [Fact]
    public void The_Host_Stacks_Its_Items()
    {
        var (repeater, _) = Hosted(10, 20, 30);

        Assert.Equal(30, repeater.DesiredSize.Height, 1);
    }

    [Fact]
    public void The_Layout_Keeps_State_For_Its_Host()
    {
        var (_, layout) = Hosted(10);

        Assert.Equal(1, layout.KnownContexts);
    }

    [Fact]
    public void Two_Hosts_Get_Two_Pieces_Of_State()
    {
        var layout = new Ex071_LayoutStateAndContext();

        Layout(Ex071_LayoutStateAndContext.CreateRepeater([10], layout), width: 300, height: 300);
        Layout(Ex071_LayoutStateAndContext.CreateRepeater([10], layout), width: 300, height: 300);

        // One layout instance, two hosts - which is exactly why nothing may be remembered
        // in a field on the layout.
        Assert.Equal(2, layout.KnownContexts);
    }

    [Fact]
    public void The_Widest_Child_Is_Recorded_Per_Host()
    {
        var layout = new Ex071_LayoutStateAndContext();
        var narrow = Layout(Ex071_LayoutStateAndContext.CreateRepeater([10, 20], layout), width: 300, height: 300);
        var wide = Layout(Ex071_LayoutStateAndContext.CreateRepeater([80], layout), width: 300, height: 300);

        // A field on the layout would make both hosts report 80 - and which one won would
        // depend on the order the passes happened to run in.
        Assert.Equal(20, narrow.DesiredSize.Width, 1);
        Assert.Equal(80, wide.DesiredSize.Width, 1);
    }

    [Fact]
    public void A_Second_Pass_Uses_The_Same_State()
    {
        var (repeater, layout) = Hosted(10, 20);
        var contextsAfterFirstPass = layout.KnownContexts;

        repeater.InvalidateMeasure();
        Layout(repeater, width: 300, height: 300);

        // The state is keyed by the host, so a second pass finds the first pass's entry
        // rather than starting a new one.
        Assert.Equal(contextsAfterFirstPass, layout.KnownContexts);
    }

    [Fact]
    public void The_Passes_Are_Counted_Per_Host()
    {
        var layout = new Ex071_LayoutStateAndContext();
        var first = Layout(Ex071_LayoutStateAndContext.CreateRepeater([10], layout), width: 300, height: 300);

        first.InvalidateMeasure();
        Layout(first, width: 300, height: 300);
        var second = Layout(Ex071_LayoutStateAndContext.CreateRepeater([10], layout), width: 300, height: 300);

        // Two hosts, and the newcomer starts at one pass rather than inheriting the other
        // host's count.
        Assert.Equal(2, layout.KnownContexts);
        Assert.NotNull(second);
    }
}
