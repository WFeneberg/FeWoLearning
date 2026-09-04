using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex035_TabsCompositionTests : BunitContext
{
    // Built with a hand-written RenderTreeBuilder (literal, non-loop sequence
    // numbers - never bUnit's AddChildContent helper) and reused with IDENTICAL
    // sequence numbers by ThreeTabsWithSecondRenamed and RemainingTwoTabs below.
    // That match is load-bearing: Blazor's diff matches child components by
    // sequence position, not just declaration order, so if the mutated fragment's
    // sequence numbers didn't line up with this one, Blazor would dispose every
    // old tab and mount brand-new instances instead of diffing them in place -
    // which would make the relabel/removal facts pass even against a registry
    // that only ever reads Title/ChildContent once at construction time.
    private IRenderedComponent<Ex035_TabsComposition> RenderThreeTabs() =>
        Render<Ex035_TabsComposition>(p => p.Add(x => x.ChildContent, (RenderFragment)ThreeTabsOriginal));

    private static void ThreeTabsOriginal(RenderTreeBuilder builder)
    {
        builder.OpenComponent<Ex035_TabsComposition_Tab>(0);
        builder.AddAttribute(1, nameof(Ex035_TabsComposition_Tab.Title), "One");
        builder.AddAttribute(2, nameof(Ex035_TabsComposition_Tab.ChildContent), (RenderFragment)(b => b.AddContent(0, "first")));
        builder.CloseComponent();

        builder.OpenComponent<Ex035_TabsComposition_Tab>(3);
        builder.AddAttribute(4, nameof(Ex035_TabsComposition_Tab.Title), "Two");
        builder.AddAttribute(5, nameof(Ex035_TabsComposition_Tab.ChildContent), (RenderFragment)(b => b.AddContent(0, "second")));
        builder.CloseComponent();

        builder.OpenComponent<Ex035_TabsComposition_Tab>(6);
        builder.AddAttribute(7, nameof(Ex035_TabsComposition_Tab.Title), "Three");
        builder.AddAttribute(8, nameof(Ex035_TabsComposition_Tab.ChildContent), (RenderFragment)(b => b.AddContent(0, "third")));
        builder.CloseComponent();
    }

    [Fact]
    public void Renders_One_Header_Per_Tab_In_Registration_Order()
    {
        var cut = RenderThreeTabs();

        // Plain initial render, no event dispatch - per rule 4 this needs no
        // WaitForAssertion.
        var headers = cut.FindAll("#tabs button.tab");
        Assert.Equal(3, headers.Count);
        Assert.Equal(new[] { "One", "Two", "Three" }, headers.Select(h => h.TextContent).ToArray());
    }

    [Fact]
    public void The_First_Tab_Is_Active_Initially_And_Its_Panel_Shows()
    {
        var cut = RenderThreeTabs();

        Assert.True(cut.Find("#tab-0").ClassList.Contains("active"));
        Assert.False(cut.Find("#tab-1").ClassList.Contains("active"));
        Assert.False(cut.Find("#tab-2").ClassList.Contains("active"));
        Assert.Equal("first", cut.Find("#tab-panel").TextContent);
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
    // pattern as-is. This fragment reuses ThreeTabsOriginal's exact sequence numbers
    // (only the second tab's Title differs), so Blazor diffs all three tabs in place
    // instead of disposing and recreating them - the tab instances survive, so this
    // can only pass if the parent actually re-reads Title live off them.
    [Fact]
    public void Relabeling_A_Tab_Updates_Its_Header_Text()
    {
        var cut = RenderThreeTabs();

        cut.Render(p => p.Add(x => x.ChildContent, (RenderFragment)ThreeTabsWithSecondRenamed));

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
    // the active one - would linger as a phantom. Activate the third tab, then drop it:
    // this fragment reuses the first two tabs' exact sequence numbers (0-2 and 3-5)
    // from ThreeTabsOriginal, so Blazor diffs tabs one and two in place (they are not
    // recreated) and disposes only the third, previously-active one - proving real
    // unregistration rather than a fresh, coincidentally-correct re-mount.
    [Fact]
    public void Removing_The_Active_Tab_Drops_Its_Header_And_Leaves_No_Stale_Active_Class()
    {
        var cut = RenderThreeTabs();
        cut.Find("#tab-2").Click();
        cut.WaitForAssertion(() => Assert.True(cut.Find("#tab-2").ClassList.Contains("active")));

        cut.Render(p => p.Add(x => x.ChildContent, (RenderFragment)RemainingTwoTabs));

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
