using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex030_ComponentCompositionTests : BunitContext
{
    [Fact]
    public void Renders_One_Crumb_Per_Registered_Item_In_Registration_Order()
    {
        var cut = Render<Ex030_ComponentComposition>(p => p
            .AddChildContent<Ex030_ComponentComposition_Item>(cp => cp.Add(x => x.Label, "Home"))
            .AddChildContent<Ex030_ComponentComposition_Item>(cp => cp.Add(x => x.Label, "Docs"))
            .AddChildContent<Ex030_ComponentComposition_Item>(cp => cp.Add(x => x.Label, "Api")));

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
        var cut = Render<Ex030_ComponentComposition>(p => p
            .AddChildContent<Ex030_ComponentComposition_Item>(cp => cp.Add(x => x.Label, "Home"))
            .AddChildContent<Ex030_ComponentComposition_Item>(cp => cp.Add(x => x.Label, "Docs"))
            .AddChildContent<Ex030_ComponentComposition_Item>(cp => cp.Add(x => x.Label, "Api")));

        cut.WaitForAssertion(() =>
        {
            var crumbs = cut.FindAll("#crumbs span.crumb");
            Assert.False(crumbs[0].ClassList.Contains("current"));
            Assert.False(crumbs[1].ClassList.Contains("current"));
            Assert.True(crumbs[2].ClassList.Contains("current"));
        });
    }

    [Fact]
    public void The_Crumbs_Nav_Text_Is_Exactly_The_Labels_Joined_By_A_Slash_Separator()
    {
        var cut = Render<Ex030_ComponentComposition>(p => p
            .AddChildContent<Ex030_ComponentComposition_Item>(cp => cp.Add(x => x.Label, "Home"))
            .AddChildContent<Ex030_ComponentComposition_Item>(cp => cp.Add(x => x.Label, "Docs"))
            .AddChildContent<Ex030_ComponentComposition_Item>(cp => cp.Add(x => x.Label, "Api")));

        // Exact equality, not Assert.Contains: a separator emitted after every crumb
        // (including the last) would produce "Home / Docs / Api / ", which still
        // *contains* "Home / Docs / Api" and would slip past a substring check.
        cut.WaitForAssertion(() => Assert.Equal("Home / Docs / Api", cut.Find("#crumbs").TextContent));
    }
}
