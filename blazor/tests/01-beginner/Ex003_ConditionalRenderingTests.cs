using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex003_ConditionalRenderingTests : BunitContext
{
    [Fact]
    public void Loading_Takes_Precedence_And_Renders_Alone()
    {
        var cut = Render<Ex003_ConditionalRendering>(p => p.Add(c => c.IsLoading, true));

        Assert.Equal("Loading", cut.Find("#loading").TextContent);
        Assert.Empty(cut.FindAll("#error"));
        Assert.Empty(cut.FindAll("#content"));
        Assert.Empty(cut.FindAll("#empty"));
    }

    [Fact]
    public void Error_Renders_Alone_When_Present()
    {
        var cut = Render<Ex003_ConditionalRendering>(p => p.Add(c => c.ErrorMessage, "boom"));

        Assert.Equal("boom", cut.Find("#error").TextContent);
        Assert.Empty(cut.FindAll("#loading"));
        Assert.Empty(cut.FindAll("#content"));
        Assert.Empty(cut.FindAll("#empty"));
    }

    [Fact]
    public void Content_Renders_Alone_When_Present()
    {
        var cut = Render<Ex003_ConditionalRendering>(p => p.Add(c => c.Content, "hi"));

        Assert.Equal("hi", cut.Find("#content").TextContent);
        Assert.Empty(cut.FindAll("#loading"));
        Assert.Empty(cut.FindAll("#error"));
        Assert.Empty(cut.FindAll("#empty"));
    }

    [Fact]
    public void Empty_Renders_Alone_When_No_Parameters_Are_Set()
    {
        var cut = Render<Ex003_ConditionalRendering>();

        Assert.Equal("No data", cut.Find("#empty").TextContent);
        Assert.Empty(cut.FindAll("#loading"));
        Assert.Empty(cut.FindAll("#error"));
        Assert.Empty(cut.FindAll("#content"));
    }

    [Fact]
    public void Loading_Wins_Over_Error_And_Content()
    {
        var cut = Render<Ex003_ConditionalRendering>(p => p
            .Add(c => c.IsLoading, true)
            .Add(c => c.ErrorMessage, "boom")
            .Add(c => c.Content, "hi"));

        Assert.Equal("Loading", cut.Find("#loading").TextContent);
        Assert.Empty(cut.FindAll("#error"));
        Assert.Empty(cut.FindAll("#content"));
        Assert.Empty(cut.FindAll("#empty"));
    }

    [Fact]
    public void Blank_Error_And_Content_Count_As_Absent()
    {
        var cut = Render<Ex003_ConditionalRendering>(p => p
            .Add(c => c.ErrorMessage, "   ")
            .Add(c => c.Content, ""));

        Assert.Equal("No data", cut.Find("#empty").TextContent);
        Assert.Empty(cut.FindAll("#loading"));
        Assert.Empty(cut.FindAll("#error"));
        Assert.Empty(cut.FindAll("#content"));
    }
}
