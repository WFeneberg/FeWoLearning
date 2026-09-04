using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex002_ParameterDefaultsTests : BunitContext
{
    [Fact]
    public void Badge_Shows_Name_Only_When_Title_Is_Absent()
    {
        var cut = Render<Ex002_ParameterDefaults>(p => p.Add(c => c.Name, "Ada"));

        Assert.Equal("Ada", cut.Find("#badge").TextContent);
    }

    [Fact]
    public void Badge_Shows_Name_And_Title_When_Title_Is_Present()
    {
        var cut = Render<Ex002_ParameterDefaults>(p => p
            .Add(c => c.Name, "Ada")
            .Add(c => c.Title, "Architect"));

        Assert.Equal("Ada (Architect)", cut.Find("#badge").TextContent);
    }

    [Fact]
    public void Badge_Treats_A_Blank_Title_As_Absent()
    {
        var cut = Render<Ex002_ParameterDefaults>(p => p
            .Add(c => c.Name, "Ada")
            .Add(c => c.Title, "   "));

        Assert.Equal("Ada", cut.Find("#badge").TextContent);
    }

    [Fact]
    public void Level_Defaults_To_One()
    {
        var cut = Render<Ex002_ParameterDefaults>(p => p.Add(c => c.Name, "Ada"));

        Assert.Equal("Level 1", cut.Find("#level").TextContent);
    }

    [Fact]
    public void Level_Reflects_The_Given_Value()
    {
        var cut = Render<Ex002_ParameterDefaults>(p => p.Add(c => c.Level, 7));

        Assert.Equal("Level 7", cut.Find("#level").TextContent);
    }
}
