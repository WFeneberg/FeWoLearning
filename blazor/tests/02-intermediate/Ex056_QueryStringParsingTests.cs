using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex056_QueryStringParsingTests : BunitContext
{
    // Navigating before the render is what puts the query on Navigation.Uri; the
    // component reads it in OnInitialized, so the order matters.
    private void GoTo(string url) => Services.GetRequiredService<NavigationManager>().NavigateTo(url);

    [Fact]
    public void Falls_Back_When_There_Is_No_Query_At_All()
    {
        var cut = Render<Ex056_QueryStringParsing>();

        Assert.Equal("1", cut.Find("#page").TextContent);
        Assert.Equal("", cut.Find("#term").TextContent);
        Assert.Equal("", cut.Find("#tags").TextContent);
    }

    [Fact]
    public void Reads_Page_And_Term()
    {
        GoTo("/search?page=3&q=blazor");

        var cut = Render<Ex056_QueryStringParsing>();

        Assert.Equal("3", cut.Find("#page").TextContent);
        Assert.Equal("blazor", cut.Find("#term").TextContent);
    }

    // Non-vacuity: an int.Parse would throw here rather than fall back, so a naive
    // implementation fails this fact instead of quietly passing the others.
    [Fact]
    public void Unparsable_Page_Falls_Back_To_One()
    {
        GoTo("/search?page=not-a-number");

        var cut = Render<Ex056_QueryStringParsing>();

        Assert.Equal("1", cut.Find("#page").TextContent);
    }

    // This is the fact that forces a real query parser: hand-rolled splitting on '?',
    // '&' and '=' passes every other fact here and leaves the escape undecoded.
    [Fact]
    public void Decodes_Percent_Escapes_In_A_Value()
    {
        GoTo("/search?q=hello%20world&page=2");

        var cut = Render<Ex056_QueryStringParsing>();

        Assert.Equal("hello world", cut.Find("#term").TextContent);
        Assert.Equal("2", cut.Find("#page").TextContent);
    }

    // A key may legally repeat; taking only the first value (or the last) loses data.
    [Fact]
    public void Keeps_Every_Value_Of_A_Repeated_Key_In_Order()
    {
        GoTo("/search?tag=red&tag=blue&tag=green");

        var cut = Render<Ex056_QueryStringParsing>();

        Assert.Equal("red,blue,green", cut.Find("#tags").TextContent);
    }
}
