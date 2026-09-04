using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex017_OnInitializedTests : BunitContext
{
    [Fact]
    public void Greeting_Is_Captured_From_The_Initial_User()
    {
        var cut = Render<Ex017_OnInitialized>(p => p.Add(c => c.User, "Ada"));

        Assert.Equal("Welcome, Ada", cut.Find("#greeting").TextContent);
    }

    [Fact]
    public void Greeting_Does_Not_Change_When_User_Changes_Afterwards()
    {
        var cut = Render<Ex017_OnInitialized>(p => p.Add(c => c.User, "Ada"));

        cut.Render(p => p.Add(c => c.User, "Grace"));

        // The whole point: OnInitialized runs once, so a later parameter
        // change must not recompute the greeting - that is ex018's job.
        Assert.Equal("Welcome, Ada", cut.Find("#greeting").TextContent);
    }
}
