using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex035_TabsCompositionTests : BunitContext
{
    private IRenderedComponent<Ex035_TabsComposition> RenderThreeTabs() => Render<Ex035_TabsComposition>(p => p
        .AddChildContent<Ex035_TabsComposition_Tab>(cp => cp.Add(x => x.Title, "One").Add(x => x.ChildContent, "first"))
        .AddChildContent<Ex035_TabsComposition_Tab>(cp => cp.Add(x => x.Title, "Two").Add(x => x.ChildContent, "second"))
        .AddChildContent<Ex035_TabsComposition_Tab>(cp => cp.Add(x => x.Title, "Three").Add(x => x.ChildContent, "third")));

    [Fact]
    public void Renders_One_Header_Per_Tab_In_Registration_Order()
    {
        var cut = RenderThreeTabs();

        cut.WaitForAssertion(() =>
        {
            var headers = cut.FindAll("#tabs button.tab");
            Assert.Equal(3, headers.Count);
            Assert.Equal(new[] { "One", "Two", "Three" }, headers.Select(h => h.TextContent).ToArray());
        });
    }

    [Fact]
    public void The_First_Tab_Is_Active_Initially_And_Its_Panel_Shows()
    {
        var cut = RenderThreeTabs();

        cut.WaitForAssertion(() =>
        {
            Assert.True(cut.Find("#tab-0").ClassList.Contains("active"));
            Assert.False(cut.Find("#tab-1").ClassList.Contains("active"));
            Assert.False(cut.Find("#tab-2").ClassList.Contains("active"));
            Assert.Equal("first", cut.Find("#tab-panel").TextContent);
        });
    }

    [Fact]
    public void Clicking_A_Header_Activates_That_Tab()
    {
        var cut = RenderThreeTabs();

        cut.Find("#tab-1").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("second", cut.Find("#tab-panel").TextContent);
            Assert.True(cut.Find("#tab-1").ClassList.Contains("active"));
            Assert.False(cut.Find("#tab-0").ClassList.Contains("active"));
        });
    }

    [Fact]
    public void Only_The_Active_Panel_Is_Rendered()
    {
        var cut = RenderThreeTabs();

        cut.Find("#tab-1").Click();

        cut.WaitForAssertion(() =>
        {
            var panels = cut.FindAll("#tab-panel");
            Assert.Single(panels);
            Assert.DoesNotContain("first", panels[0].TextContent);
        });
    }

    // Ruling A, fact 1 of 2: ex030's registry captures Label by value at registration
    // time and never re-reads it, so a relabel would never reach the header. Title is
    // a live [Parameter] here, and this is the whole reason ex035 cannot reuse that
    // pattern as-is. A relabel can only reach the tree the way a real app would cause
    // one: the parent re-rendering with a new Title for that tab (confirmed by probing
    // bUnit directly: pushing a parameter via FindComponents<T>()[i].Render(...) does
    // NOT reach the live instance embedded in the tree - Instance.Title stayed
    // unchanged - so it cannot be used to simulate this).
    [Fact]
    public void Relabeling_A_Tab_Updates_Its_Header_Text()
    {
        var cut = RenderThreeTabs();

        cut.Render(p => p.Add(x => x.ChildContent, ThreeTabsWithSecondRenamed));

        var headers = cut.FindAll("#tabs button.tab");
        Assert.Equal(new[] { "One", "Two-Renamed", "Three" }, headers.Select(h => h.TextContent).ToArray());
    }

    private static void ThreeTabsWithSecondRenamed(RenderTreeBuilder builder)
    {
        builder.OpenComponent<Ex035_TabsComposition_Tab>(0);
        builder.AddAttribute(1, nameof(Ex035_TabsComposition_Tab.Title), "One");
        builder.AddAttribute(2, nameof(Ex035_TabsComposition_Tab.ChildContent), (RenderFragment)(b => b.AddContent(0, "first")));
        builder.CloseComponent();

        builder.OpenComponent<Ex035_TabsComposition_Tab>(3);
        builder.AddAttribute(4, nameof(Ex035_TabsComposition_Tab.Title), "Two-Renamed");
        builder.AddAttribute(5, nameof(Ex035_TabsComposition_Tab.ChildContent), (RenderFragment)(b => b.AddContent(0, "second")));
        builder.CloseComponent();

        builder.OpenComponent<Ex035_TabsComposition_Tab>(6);
        builder.AddAttribute(7, nameof(Ex035_TabsComposition_Tab.Title), "Three");
        builder.AddAttribute(8, nameof(Ex035_TabsComposition_Tab.ChildContent), (RenderFragment)(b => b.AddContent(0, "third")));
        builder.CloseComponent();
    }

    // Ruling A, fact 2 of 2: ex030's registered items are never unregistered (they are
    // not IDisposable), so a removed tab's header - and its "active" class if it was
    // the active one - would linger as a phantom. Activate the third tab, then remove
    // it, and prove neither the header nor its active-ness survives. Dropping a tab can
    // only happen by giving the parent new ChildContent (a child cannot unmount itself),
    // so this rebuilds it explicitly with fixed sequence numbers - not a loop - to stay
    // clear of analyzer ASP0006.
    [Fact]
    public void Removing_The_Active_Tab_Drops_Its_Header_And_Leaves_No_Stale_Active_Class()
    {
        var cut = RenderThreeTabs();
        cut.Find("#tab-2").Click();
        cut.WaitForAssertion(() => Assert.True(cut.Find("#tab-2").ClassList.Contains("active")));

        cut.Render(p => p.Add(x => x.ChildContent, RemainingTwoTabs));

        var headers = cut.FindAll("#tabs button.tab");
        Assert.Equal(new[] { "One", "Two" }, headers.Select(h => h.TextContent).ToArray());
        Assert.Single(headers, h => h.ClassList.Contains("active"));
    }

    private static void RemainingTwoTabs(RenderTreeBuilder builder)
    {
        builder.OpenComponent<Ex035_TabsComposition_Tab>(0);
        builder.AddAttribute(1, nameof(Ex035_TabsComposition_Tab.Title), "One");
        builder.AddAttribute(2, nameof(Ex035_TabsComposition_Tab.ChildContent), (RenderFragment)(b => b.AddContent(0, "first")));
        builder.CloseComponent();

        builder.OpenComponent<Ex035_TabsComposition_Tab>(3);
        builder.AddAttribute(4, nameof(Ex035_TabsComposition_Tab.Title), "Two");
        builder.AddAttribute(5, nameof(Ex035_TabsComposition_Tab.ChildContent), (RenderFragment)(b => b.AddContent(0, "second")));
        builder.CloseComponent();
    }
}
