using Bunit;
using Bunit.TestDoubles;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex033_NavigationManagerOpenRedirectTests
{
    // BunitNavigationManager.History is newest-first and has no indexer -
    // History.First() is the entry GoTo just produced.
    [Theory]
    [InlineData("https://evil.example/")]
    [InlineData("//evil.example/")]
    [InlineData("javascript:alert(1)")]
    [InlineData(null)]
    public void Attack_Unsafe_Candidate_Navigates_To_The_Apps_Own_Root(string? candidate)
    {
        using var harness = new BlazorHarness();
        var navigation = (BunitNavigationManager)harness.Services.GetRequiredService<NavigationManager>();

        Ex033_NavigationManagerOpenRedirect.GoTo(navigation, candidate);

        Assert.Equal("/", navigation.History.First().Uri);
    }

    [Fact]
    public void Use_A_Local_Path_Navigates_There_Unchanged()
    {
        using var harness = new BlazorHarness();
        var navigation = (BunitNavigationManager)harness.Services.GetRequiredService<NavigationManager>();

        Ex033_NavigationManagerOpenRedirect.GoTo(navigation, "/dashboard");

        Assert.Equal("/dashboard", navigation.History.First().Uri);
    }

    [Fact]
    public void Use_A_Local_Path_With_A_Query_Preserves_The_Query()
    {
        using var harness = new BlazorHarness();
        var navigation = (BunitNavigationManager)harness.Services.GetRequiredService<NavigationManager>();

        Ex033_NavigationManagerOpenRedirect.GoTo(navigation, "/reports?year=2026");

        Assert.Equal("/reports?year=2026", navigation.History.First().Uri);
    }
}
