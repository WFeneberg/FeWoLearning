using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex015_DynamicCssClassTests : BunitContext
{
    [Fact]
    public void Info_Severity_Produces_The_Info_Class()
    {
        var cut = Render<Ex015_DynamicCssClass>(p => p.Add(c => c.Severity, AlertSeverity.Info));

        Assert.Equal("alert alert-info", cut.Find("#alert").GetAttribute("class"));
    }

    [Fact]
    public void Warning_Severity_Produces_The_Warning_Class()
    {
        var cut = Render<Ex015_DynamicCssClass>(p => p.Add(c => c.Severity, AlertSeverity.Warning));

        Assert.Equal("alert alert-warning", cut.Find("#alert").GetAttribute("class"));
    }

    [Fact]
    public void Danger_Severity_Produces_The_Danger_Class()
    {
        var cut = Render<Ex015_DynamicCssClass>(p => p.Add(c => c.Severity, AlertSeverity.Danger));

        Assert.Equal("alert alert-danger", cut.Find("#alert").GetAttribute("class"));
    }

    [Fact]
    public void Dismissed_Appends_Its_Class_After_The_Severity_Class()
    {
        var cut = Render<Ex015_DynamicCssClass>(p => p.Add(c => c.Severity, AlertSeverity.Danger));

        // Pre-state sanity, folded in here rather than as a standalone fact: with
        // Dismissed left at its default (false), the class list must not already
        // contain alert-dismissed. On its own this would pass under almost any
        // implementation, since nothing has added the suffix yet - only the
        // assertion after the re-render below, which actually flips Dismissed,
        // exercises the dismissed-handling branch of the TODO.
        Assert.DoesNotContain("alert-dismissed", cut.Find("#alert").ClassList);

        cut.Render(p => p.Add(c => c.Severity, AlertSeverity.Danger).Add(c => c.Dismissed, true));

        Assert.Equal("alert alert-danger alert-dismissed", cut.Find("#alert").GetAttribute("class"));
    }
}
