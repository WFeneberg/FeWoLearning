using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex027_RadioGroupTests : BunitContext
{
    [Fact]
    public void Renders_One_Radio_Per_Size_With_None_Checked_And_Chosen_Empty()
    {
        var cut = Render<Ex027_RadioGroup>(p => p.Add(c => c.Sizes, new[] { "S", "M", "L" }));

        Assert.Equal(3, cut.FindAll("input[type=radio]").Count);
        Assert.Equal("", cut.Find("#chosen").TextContent);
        Assert.False(cut.Find("#size-0").HasAttribute("checked"));
        Assert.False(cut.Find("#size-1").HasAttribute("checked"));
        Assert.False(cut.Find("#size-2").HasAttribute("checked"));
    }

    [Fact]
    public void An_Empty_Sizes_List_Renders_No_Radios()
    {
        // Every other fact uses the same fixed Sizes array, so this is what
        // actually rules out a hard-coded set of radios rather than a real
        // projection over Sizes - see ex025's identical empty-list fact.
        var cut = Render<Ex027_RadioGroup>(p => p.Add(c => c.Sizes, Array.Empty<string>()));

        Assert.Empty(cut.FindAll("input[type=radio]"));
        Assert.Equal("", cut.Find("#chosen").TextContent);
    }

    [Fact]
    public void Selecting_A_Radio_Checks_It_And_Updates_Chosen()
    {
        var cut = Render<Ex027_RadioGroup>(p => p.Add(c => c.Sizes, new[] { "S", "M", "L" }));

        cut.Find("#size-1").Change(true);

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("M", cut.Find("#chosen").TextContent);
            Assert.True(cut.Find("#size-1").HasAttribute("checked"));
            Assert.False(cut.Find("#size-0").HasAttribute("checked"));
            Assert.False(cut.Find("#size-2").HasAttribute("checked"));
        });
    }

    [Fact]
    public void Selecting_A_Different_Radio_Clears_The_Previous_One()
    {
        var cut = Render<Ex027_RadioGroup>(p => p.Add(c => c.Sizes, new[] { "S", "M", "L" }));

        cut.Find("#size-1").Change(true);
        cut.Find("#size-2").Change(true);

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("L", cut.Find("#chosen").TextContent);
            Assert.False(cut.Find("#size-1").HasAttribute("checked"));
        });
    }
}
