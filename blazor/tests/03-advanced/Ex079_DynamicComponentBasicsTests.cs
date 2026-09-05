using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

public class Ex079_DynamicComponentBasicsTests : BunitContext
{
    private static Dictionary<string, object> Badge(string label, int count)
        => new() { ["Label"] = label, ["Count"] = count };

    [Fact]
    public void Renders_The_Type_It_Was_Given()
    {
        var cut = Render<Ex079_DynamicComponentBasics>(p => p
            .Add(c => c.ComponentType, typeof(DynamicBadge))
            .Add(c => c.Parameters, Badge("ready", 3)));

        Assert.Equal("ready", cut.Find(".badge").TextContent);
        Assert.Equal("3", cut.Find(".badge").GetAttribute("data-count"));
    }

    // A second type, so "rendered what it was told to" is distinguishable from
    // "rendered the only thing it can".
    [Fact]
    public void A_Different_Type_Renders_A_Different_Component()
    {
        var cut = Render<Ex079_DynamicComponentBasics>(p => p
            .Add(c => c.ComponentType, typeof(DynamicNote))
            .Add(c => c.Parameters, new Dictionary<string, object> { ["Text"] = "hello" }));

        Assert.Equal("hello", cut.Find(".note").TextContent);
        Assert.Empty(cut.FindAll(".badge"));
    }

    [Fact]
    public void Switching_The_Type_Swaps_What_Is_Rendered()
    {
        var cut = Render<Ex079_DynamicComponentBasics>(p => p
            .Add(c => c.ComponentType, typeof(DynamicBadge))
            .Add(c => c.Parameters, Badge("ready", 3)));

        cut.Render(p => p
            .Add(c => c.ComponentType, typeof(DynamicNote))
            .Add(c => c.Parameters, new Dictionary<string, object> { ["Text"] = "hello" }));

        Assert.Empty(cut.FindAll(".badge"));
        Assert.Equal("hello", cut.Find(".note").TextContent);
    }

    // Instance is the component DynamicComponent actually built - the same object
    // that is in the render tree, which is what makes it usable for calling into.
    [Fact]
    public void Instance_Is_The_Component_That_Was_Built()
    {
        var cut = Render<Ex079_DynamicComponentBasics>(p => p
            .Add(c => c.ComponentType, typeof(DynamicBadge))
            .Add(c => c.Parameters, Badge("ready", 3)));

        var built = Assert.IsType<DynamicBadge>(cut.Instance.Rendered);
        Assert.Same(cut.FindComponent<DynamicBadge>().Instance, built);
        Assert.Equal("ready", built.Label);
    }
}
