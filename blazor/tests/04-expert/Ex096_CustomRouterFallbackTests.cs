using Bunit;
using FeWoLearning.Blazor.Exercises.Expert;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Expert;

using Choice = Ex096_CustomRouterFallback.Choice;

public class Ex096_CustomRouterFallbackTests : BunitContext
{
    private static RenderFragment Page(string id) => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "page");
        builder.AddAttribute(2, "data-page", id);
        builder.CloseElement();
    };

    private static readonly Dictionary<string, RenderFragment> Routes = new()
    {
        ["about"] = Page("about"),
        ["contact"] = Page("contact"),
    };

    private void GoTo(string url) => Services.GetRequiredService<NavigationManager>().NavigateTo(url);

    private IRenderedComponent<Ex096_CustomRouterFallback> RenderRouter(
        bool withCatchAll = false, bool withNotFound = false)
        => Render<Ex096_CustomRouterFallback>(p =>
        {
            p.Add(c => c.Routes, Routes);
            if (withCatchAll)
            {
                p.Add(c => c.CatchAll, Page("catchall"));
            }
            if (withNotFound)
            {
                p.Add(c => c.NotFound, Page("notfound"));
            }
        });

    // Ruling: order first. A router that consults its fallbacks before its routes
    // answers this with the catch-all and still looks correct on an unknown path,
    // so both fallbacks are present here on purpose.
    [Fact]
    public void A_Known_Path_Takes_Its_Route_Even_When_Fallbacks_Exist()
    {
        GoTo("/about");

        var cut = RenderRouter(withCatchAll: true, withNotFound: true);

        Assert.Equal("about", cut.Find(".page").GetAttribute("data-page"));
        Assert.Equal(Choice.Route, cut.Instance.Chosen);
    }

    [Fact]
    public void An_Unknown_Path_Falls_To_The_Catch_All_Before_Not_Found()
    {
        GoTo("/nowhere");

        var cut = RenderRouter(withCatchAll: true, withNotFound: true);

        Assert.Equal("catchall", cut.Find(".page").GetAttribute("data-page"));
        Assert.Equal(Choice.CatchAll, cut.Instance.Chosen);
    }

    [Fact]
    public void Without_A_Catch_All_It_Falls_To_Not_Found()
    {
        GoTo("/nowhere");

        var cut = RenderRouter(withNotFound: true);

        Assert.Equal("notfound", cut.Find(".page").GetAttribute("data-page"));
        Assert.Equal(Choice.NotFound, cut.Instance.Chosen);
    }

    // Rendering nothing is a legitimate outcome, not a crash and not a blank page
    // element. Negative assertion, so it stays bare.
    [Fact]
    public void With_Neither_Fallback_It_Renders_Nothing()
    {
        GoTo("/nowhere");

        var cut = RenderRouter();

        Assert.Empty(cut.FindAll(".page"));
        Assert.Equal(Choice.Nothing, cut.Instance.Chosen);
    }

    [Fact]
    public void A_Navigation_Re_Decides()
    {
        GoTo("/nowhere");
        var cut = RenderRouter(withCatchAll: true);
        Assert.Equal(Choice.CatchAll, cut.Instance.Chosen);

        GoTo("/contact");

        cut.WaitForAssertion(() => Assert.Equal("contact", cut.Find(".page").GetAttribute("data-page")));
        Assert.Equal(Choice.Route, cut.Instance.Chosen);
    }

    [Fact]
    public void A_Query_String_Does_Not_Hide_A_Route()
    {
        GoTo("/about?tab=team");

        var cut = RenderRouter(withCatchAll: true);

        Assert.Equal("about", cut.Find(".page").GetAttribute("data-page"));
    }
}
