using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex018_OnParametersSetTests : BunitContext
{
    [Fact]
    public void Title_Is_Slugified_Lowercase_With_Spaces_As_Hyphens()
    {
        var cut = Render<Ex018_OnParametersSet>(p => p.Add(c => c.Title, "Hello Blazor World"));

        Assert.Equal("hello-blazor-world", cut.Find("#slug").TextContent);
    }

    [Fact]
    public void Slug_Recomputes_When_Title_Changes()
    {
        var cut = Render<Ex018_OnParametersSet>(p => p.Add(c => c.Title, "Hello Blazor World"));

        cut.Render(p => p.Add(c => c.Title, "Second Title"));

        // Rejects an OnInitialized-only implementation: the slug must track
        // every parameter change, not just the first one.
        Assert.Equal("second-title", cut.Find("#slug").TextContent);
    }

    [Fact]
    public void Non_Alphanumeric_Characters_Are_Dropped_And_Hyphen_Runs_Collapsed()
    {
        var cut = Render<Ex018_OnParametersSet>(p => p.Add(c => c.Title, "A -- B!"));

        // Rejects a naive Replace(" ", "-").ToLower(): that would leave the
        // "--" run and the "!" character in place instead of collapsing/dropping them.
        Assert.Equal("a-b", cut.Find("#slug").TextContent);
    }
}
