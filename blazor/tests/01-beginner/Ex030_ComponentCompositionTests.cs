using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex030_ComponentCompositionTests : BunitContext
{
    private static RenderFragment ThreeItems() => builder =>
    {
        builder.OpenComponent<Ex030_ComponentComposition_Item>(0);
        builder.AddAttribute(1, "Label", "Home");
        builder.CloseComponent();

        builder.OpenComponent<Ex030_ComponentComposition_Item>(2);
        builder.AddAttribute(3, "Label", "Docs");
        builder.CloseComponent();

        builder.OpenComponent<Ex030_ComponentComposition_Item>(4);
        builder.AddAttribute(5, "Label", "Api");
        builder.CloseComponent();
    };

    [Fact]
    public void Renders_One_Crumb_Per_Registered_Item_In_Registration_Order()
    {
        var cut = Render<Ex030_ComponentComposition>(p => p.Add(c => c.ChildContent, ThreeItems()));

        cut.WaitForAssertion(() =>
        {
            var crumbs = cut.FindAll("#crumbs span.crumb");
            Assert.Equal(3, crumbs.Count);
            Assert.Equal(new[] { "Home", "Docs", "Api" }, crumbs.Select(c => c.TextContent).ToArray());
        });
    }

    [Fact]
    public void Only_The_Last_Crumb_Carries_The_Current_Class()
    {
        var cut = Render<Ex030_ComponentComposition>(p => p.Add(c => c.ChildContent, ThreeItems()));

        cut.WaitForAssertion(() =>
        {
            var crumbs = cut.FindAll("#crumbs span.crumb");
            Assert.False(crumbs[0].ClassList.Contains("current"));
            Assert.False(crumbs[1].ClassList.Contains("current"));
            Assert.True(crumbs[2].ClassList.Contains("current"));
        });
    }

    [Fact]
    public void The_Crumbs_Nav_Text_Joins_Labels_With_A_Slash_Separator()
    {
        var cut = Render<Ex030_ComponentComposition>(p => p.Add(c => c.ChildContent, ThreeItems()));

        cut.WaitForAssertion(() => Assert.Contains("Home / Docs / Api", cut.Find("#crumbs").TextContent));
    }
}
