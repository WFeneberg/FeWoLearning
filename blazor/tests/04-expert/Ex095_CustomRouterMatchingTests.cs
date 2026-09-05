using Bunit;
using FeWoLearning.Blazor.Exercises.Expert;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Expert;

using RouteEntry = Ex095_CustomRouterMatching.RouteEntry;

public class Ex095_CustomRouterMatchingTests : BunitContext
{
    private static RouteEntry Route(string pattern, string id)
        => new(pattern, values => builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "page");
            builder.AddAttribute(2, "data-page", id);
            builder.AddContent(3, string.Join(",", values.Select(v => $"{v.Key}={v.Value}")));
            builder.CloseElement();
        });

    private void GoTo(string url) => Services.GetRequiredService<NavigationManager>().NavigateTo(url);

    private IRenderedComponent<Ex095_CustomRouterMatching> RenderRouter(params RouteEntry[] routes)
        => Render<Ex095_CustomRouterMatching>(p => p.Add(c => c.Routes, routes));

    [Fact]
    public void A_Literal_Route_Matches_Its_Path()
    {
        GoTo("/about");

        var cut = RenderRouter(Route("/home", "home"), Route("/about", "about"));

        Assert.Equal("about", cut.Find(".page").GetAttribute("data-page"));
    }

    [Fact]
    public void A_Parameter_Segment_Takes_Its_Value_Out_Of_The_Path()
    {
        GoTo("/users/42");

        var cut = RenderRouter(Route("/users/{id}", "user"));

        Assert.Equal("user", cut.Find(".page").GetAttribute("data-page"));
        Assert.Equal("id=42", cut.Find(".page").TextContent);
        Assert.Equal("42", cut.Instance.RouteValues["id"]);
    }

    // Ruling: the constraint has to reject, not merely annotate. The int route comes
    // first, so a matcher that ignores ":int" answers with it and this fact fails on
    // the page id.
    [Fact]
    public void An_Int_Constraint_Rejects_A_Non_Numeric_Segment()
    {
        GoTo("/users/ada");

        var cut = RenderRouter(Route("/users/{id:int}", "by-id"), Route("/users/{name}", "by-name"));

        Assert.Equal("by-name", cut.Find(".page").GetAttribute("data-page"));
        Assert.Equal("ada", cut.Instance.RouteValues["name"]);
    }

    [Fact]
    public void An_Int_Constraint_Still_Accepts_A_Number()
    {
        GoTo("/users/42");

        var cut = RenderRouter(Route("/users/{id:int}", "by-id"), Route("/users/{name}", "by-name"));

        Assert.Equal("by-id", cut.Find(".page").GetAttribute("data-page"));
    }

    // A route matches the whole path or not at all - neither a prefix of it nor an
    // extension of it. Negative assertions, so they stay bare.
    [Theory]
    [InlineData("/users")]
    [InlineData("/users/42/edit")]
    public void A_Route_Does_Not_Match_A_Different_Number_Of_Segments(string url)
    {
        GoTo(url);

        var cut = RenderRouter(Route("/users/{id}", "user"));

        Assert.Empty(cut.FindAll(".page"));
        Assert.Null(cut.Instance.Matched);
    }

    [Fact]
    public void A_Navigation_Re_Matches_The_New_Path()
    {
        GoTo("/users/42");
        var cut = RenderRouter(Route("/users/{id}", "user"), Route("/about", "about"));
        Assert.Equal("user", cut.Find(".page").GetAttribute("data-page"));

        GoTo("/about");

        cut.WaitForAssertion(() => Assert.Equal("about", cut.Find(".page").GetAttribute("data-page")));
    }

    [Fact]
    public void A_Query_String_Is_Not_Part_Of_The_Path()
    {
        GoTo("/users/42?tab=profile");

        var cut = RenderRouter(Route("/users/{id}", "user"));

        Assert.Equal("42", cut.Instance.RouteValues["id"]);
    }
}
