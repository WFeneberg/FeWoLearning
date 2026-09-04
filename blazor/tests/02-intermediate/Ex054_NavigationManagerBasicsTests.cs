using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex054_NavigationManagerBasicsTests : BunitContext
{
    [Fact]
    public void Start_Is_Not_Empty_After_Render()
    {
        var cut = Render<Ex054_NavigationManagerBasics>();

        Assert.False(string.IsNullOrEmpty(cut.Find("#start").TextContent));
    }

    [Fact]
    public void Clicking_Go_Navigates_To_Target()
    {
        var cut = Render<Ex054_NavigationManagerBasics>(p => p.Add(c => c.Target, "/somewhere?x=1"));

        cut.Find("#go").Click();

        var navigation = Services.GetRequiredService<NavigationManager>();
        var expectedUri = new Uri(new Uri(navigation.BaseUri), "/somewhere?x=1").ToString();
        Assert.Equal(expectedUri, navigation.Uri);
    }

    // Non-vacuity: binding the markup straight to @Navigation.Uri instead of a value
    // captured once in OnInitialized would show the *post-navigation* URI here -
    // verified directly by making exactly that change to the solution.
    [Fact]
    public void Start_Still_Shows_The_Original_Uri_After_Navigating()
    {
        var cut = Render<Ex054_NavigationManagerBasics>(p => p.Add(c => c.Target, "/somewhere?x=1"));
        var originalStart = cut.Find("#start").TextContent;

        cut.Find("#go").Click();

        Assert.Equal(originalStart, cut.Find("#start").TextContent);
    }
}
