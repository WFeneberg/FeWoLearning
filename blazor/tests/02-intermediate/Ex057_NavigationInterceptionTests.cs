using Bunit;
using Bunit.TestDoubles;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex057_NavigationInterceptionTests : BunitContext
{
    private BunitNavigationManager Navigation
        => (BunitNavigationManager)Services.GetRequiredService<NavigationManager>();

    // bUnit's History is stack-ordered - the first element is the most recent
    // navigation - and the initial Uri is not in it at all.
    private NavigationState LastOutcome => Navigation.History.First().State;

    [Fact]
    public void Clean_Component_Lets_Navigation_Through()
    {
        var cut = Render<Ex057_NavigationInterception>();

        Navigation.NavigateTo("/away");

        Assert.Equal(NavigationState.Succeeded, LastOutcome);
        Assert.EndsWith("/away", Navigation.Uri);
        Assert.Equal(0, cut.Instance.PreventedCount);
    }

    [Fact]
    public void Dirty_Component_Cancels_The_Navigation()
    {
        var cut = Render<Ex057_NavigationInterception>();
        cut.Find("#edit").Click();
        cut.WaitForAssertion(() => Assert.Equal("dirty", cut.Find("#state").TextContent));
        var before = Navigation.Uri;

        Navigation.NavigateTo("/away");

        Assert.Equal(NavigationState.Prevented, LastOutcome);
        Assert.Equal(before, Navigation.Uri);
        Assert.Equal(1, cut.Instance.PreventedCount);
    }

    // Non-vacuity: a handler that disposes its own registration on the first block (or
    // is registered one-shot) passes the fact above and fails here.
    [Fact]
    public void Handler_Stays_Registered_After_A_Block()
    {
        var cut = Render<Ex057_NavigationInterception>();
        cut.Find("#edit").Click();
        cut.WaitForAssertion(() => Assert.Equal("dirty", cut.Find("#state").TextContent));

        Navigation.NavigateTo("/away");
        Navigation.NavigateTo("/elsewhere");

        Assert.Equal(NavigationState.Prevented, LastOutcome);
        Assert.Equal(2, cut.Instance.PreventedCount);
    }

    // Ruling: capture the instance before disposal, then navigate while still dirty.
    // A Dispose() that does not give the registration back leaves the handler armed
    // for the rest of the circuit's life, so this navigation would still be Prevented
    // - verified directly. The PreventedCount assertion is a negative one ("did not
    // advance") and stays bare, per README section 11.
    [Fact]
    public async Task Disposed_Component_Stops_Intercepting()
    {
        var cut = Render<Ex057_NavigationInterception>();
        cut.Find("#edit").Click();
        cut.WaitForAssertion(() => Assert.Equal("dirty", cut.Find("#state").TextContent));
        var instance = cut.Instance;

        Navigation.NavigateTo("/away");
        Assert.Equal(1, instance.PreventedCount);

        await DisposeComponentsAsync();
        Navigation.NavigateTo("/away");

        Assert.Equal(NavigationState.Succeeded, LastOutcome);
        Assert.EndsWith("/away", Navigation.Uri);
        Assert.Equal(1, instance.PreventedCount);
    }
}
