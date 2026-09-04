using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex055_NavigationLocationChangedTests : BunitContext
{
    [Fact]
    public void Changes_Starts_At_Zero()
    {
        var cut = Render<Ex055_NavigationLocationChanged>();

        Assert.Equal("0", cut.Find("#changes").TextContent);
    }

    [Fact]
    public void Navigating_Increments_Changes()
    {
        var cut = Render<Ex055_NavigationLocationChanged>();
        var navigation = Services.GetRequiredService<NavigationManager>();

        navigation.NavigateTo("/elsewhere");

        cut.WaitForAssertion(() => Assert.Equal("1", cut.Find("#changes").TextContent));
    }

    // Ruling: capture the instance before disposal, dispose, then navigate again and
    // assert the captured instance's LocationChanges did not advance. Non-vacuity: a
    // Dispose() that does not unsubscribe (or unsubscribes a different delegate
    // instance than the one OnInitialized subscribed) leaves the handler live, so
    // LocationChanges would advance to 2 here instead of staying at 1 - verified
    // directly. Kept bare (no WaitForAssertion) - this is a "stayed the same"
    // assertion, per README §11's negative-assertion exemption.
    [Fact]
    public async Task Disposing_Stops_Counting_Location_Changes()
    {
        var cut = Render<Ex055_NavigationLocationChanged>();
        var navigation = Services.GetRequiredService<NavigationManager>();
        var instance = cut.Instance;

        navigation.NavigateTo("/elsewhere");
        cut.WaitForAssertion(() => Assert.Equal(1, instance.LocationChanges));

        await DisposeComponentsAsync();
        navigation.NavigateTo("/somewhere-else");

        Assert.Equal(1, instance.LocationChanges);
    }
}
