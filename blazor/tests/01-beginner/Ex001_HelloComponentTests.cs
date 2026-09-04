using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex001_HelloComponentTests : BunitContext
{
    [Fact]
    public void Greets_The_Default_Name()
    {
        var cut = Render<Ex001_HelloComponent>();

        Assert.Equal("Hello, world!", cut.Find("#greeting").TextContent);
    }

    [Fact]
    public void Greets_The_Given_Name()
    {
        var cut = Render<Ex001_HelloComponent>(p => p.Add(c => c.Name, "Blazor"));

        Assert.Equal("Hello, Blazor!", cut.Find("#greeting").TextContent);
    }
}
