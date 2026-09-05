using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

// AddSupplyValueFromQueryProvider is what supplies these values; outside a Router
// nothing else does, and the properties would silently keep their defaults.
public class Ex090_SupplyParameterFromQueryCapstoneTests : BunitContext
{
    public Ex090_SupplyParameterFromQueryCapstoneTests()
        => Services.AddSupplyValueFromQueryProvider();

    private void GoTo(string url) => Services.GetRequiredService<NavigationManager>().NavigateTo(url);

    [Fact]
    public void An_Empty_Query_Leaves_The_Defaults_Alone()
    {
        var cut = Render<Ex090_SupplyParameterFromQueryCapstone>();

        Assert.Equal("1", cut.Find("#page").TextContent);
        Assert.Equal("", cut.Find("#term").TextContent);
        Assert.Equal("", cut.Find("#tags").TextContent);
    }

    [Fact]
    public void Values_Come_Out_Of_The_Query_Typed()
    {
        GoTo("/search?page=4&q=blazor");

        var cut = Render<Ex090_SupplyParameterFromQueryCapstone>();

        Assert.Equal("4", cut.Find("#page").TextContent);
        Assert.Equal("blazor", cut.Find("#term").TextContent);
    }

    // The renamed key: the property is Term, the query says q. A property-name match
    // would leave this empty.
    [Fact]
    public void A_Renamed_Key_Still_Finds_Its_Property()
    {
        GoTo("/search?q=only-this");

        var cut = Render<Ex090_SupplyParameterFromQueryCapstone>();

        Assert.Equal("only-this", cut.Find("#term").TextContent);
    }

    // A repeated key needs an array-typed property; a string one would take a single
    // value and drop the rest.
    [Fact]
    public void A_Repeated_Key_Fills_An_Array_Property()
    {
        GoTo("/search?tag=red&tag=blue&tag=green");

        var cut = Render<Ex090_SupplyParameterFromQueryCapstone>();

        Assert.Equal("red,blue,green", cut.Find("#tags").TextContent);
    }

    // Ruling: the capstone part. These are parameters, not a one-off read, so a
    // navigation re-supplies them to a component that is already on screen - no
    // subscription and no manual re-parse, which is what ex056 had to do by hand.
    [Fact]
    public void A_Navigation_Re_Supplies_Them_To_The_Live_Component()
    {
        GoTo("/search?page=1&q=first");
        var cut = Render<Ex090_SupplyParameterFromQueryCapstone>();
        Assert.Equal("first", cut.Find("#term").TextContent);
        var appliedBefore = int.Parse(cut.Find("#applied").TextContent);

        GoTo("/search?page=9&q=second&tag=x");

        cut.WaitForAssertion(() => Assert.Equal("second", cut.Find("#term").TextContent));
        Assert.Equal("9", cut.Find("#page").TextContent);
        Assert.Equal("x", cut.Find("#tags").TextContent);
        Assert.True(int.Parse(cut.Find("#applied").TextContent) > appliedBefore);
    }
}
