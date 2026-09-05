using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

public class Ex080_DynamicComponentParameterMappingTests : BunitContext
{
    private IRenderedComponent<Ex080_DynamicComponentParameterMapping> RenderWith(
        params (string Name, string Value)[] raw)
        => Render<Ex080_DynamicComponentParameterMapping>(p => p
            .Add(c => c.ComponentType, typeof(DynamicBadge))
            .Add(c => c.RawValues, raw.ToDictionary(e => e.Name, e => e.Value)));

    [Fact]
    public void Converts_Each_Value_To_Its_Declared_Parameter_Type()
    {
        var cut = RenderWith(("Label", "ready"), ("Count", "42"));

        Assert.Equal("ready", cut.Find(".badge").TextContent);
        Assert.Equal("42", cut.Find(".badge").GetAttribute("data-count"));
        Assert.Empty(cut.Instance.Problems);
    }

    // Ruling: DynamicComponent throws on a name the target does not declare, so
    // "passed it through anyway" is not a survivable outcome - the render would fail
    // rather than the entry being reported. The badge still has to appear.
    [Fact]
    public void An_Unknown_Name_Is_Reported_And_Left_Out()
    {
        var cut = RenderWith(("Label", "ready"), ("Nonsense", "x"));

        Assert.Equal("ready", cut.Find(".badge").TextContent);
        Assert.Equal(
            [$"Nonsense: {Ex080_DynamicComponentParameterMapping.UnknownParameter}"],
            cut.Instance.Problems);
    }

    // The validating half of the row: DynamicBadge.Secret is public and settable, but
    // carries no [Parameter], so it is not addressable this way. A mapper that only
    // checks GetProperty(name) != null lets it through and DynamicComponent throws.
    [Fact]
    public void A_Public_Property_That_Is_Not_A_Parameter_Counts_As_Unknown()
    {
        var cut = RenderWith(("Label", "ready"), ("Secret", "hunter2"));

        Assert.Equal("ready", cut.Find(".badge").TextContent);
        Assert.Equal(
            [$"Secret: {Ex080_DynamicComponentParameterMapping.UnknownParameter}"],
            cut.Instance.Problems);
    }

    [Fact]
    public void A_Value_That_Does_Not_Convert_Is_Reported_And_The_Rest_Still_Lands()
    {
        var cut = RenderWith(("Label", "ready"), ("Count", "not-a-number"));

        Assert.Equal("ready", cut.Find(".badge").TextContent);
        Assert.Equal("0", cut.Find(".badge").GetAttribute("data-count"));
        Assert.Equal(
            [$"Count: {Ex080_DynamicComponentParameterMapping.NotConvertible}"],
            cut.Instance.Problems);
    }

    [Fact]
    public void Re_Mapping_Replaces_The_Previous_Problems()
    {
        var cut = RenderWith(("Nonsense", "x"));
        Assert.Single(cut.Instance.Problems);

        cut.Render(p => p
            .Add(c => c.ComponentType, typeof(DynamicBadge))
            .Add(c => c.RawValues, new Dictionary<string, string> { ["Label"] = "fine" }));

        Assert.Empty(cut.Instance.Problems);
        Assert.Equal("fine", cut.Find(".badge").TextContent);
    }
}
