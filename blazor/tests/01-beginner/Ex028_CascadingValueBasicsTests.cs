using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex028_CascadingValueBasicsTests : BunitContext
{
    [Fact]
    public void The_Label_Consumes_The_Cascaded_Theme_From_Its_Provider()
    {
        var cut = Render<Ex028_CascadingValueBasics>(p => p
            .Add(c => c.Theme, "dark")
            .AddChildContent<Ex028_CascadingValueBasics_Label>(cp => cp.Add(c => c.Text, "hi")));

        var span = cut.Find("#themed");
        Assert.Equal("theme-dark", span.ClassName);
        Assert.Equal("hi", span.TextContent);
    }

    [Fact]
    public void The_Label_Falls_Back_To_None_Without_A_Provider()
    {
        var cut = Render<Ex028_CascadingValueBasics_Label>(p => p.Add(c => c.Text, "hi"));

        Assert.Equal("theme-none", cut.Find("#themed").ClassName);
    }
}
