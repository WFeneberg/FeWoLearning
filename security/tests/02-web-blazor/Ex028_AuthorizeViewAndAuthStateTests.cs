using System.Security.Claims;
using Bunit;
using FeWoLearning.Security.Exercises.Support;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex028_AuthorizeViewAndAuthStateTests
{
    private static BlazorHarness CreateHarness(ClaimsPrincipal principal)
    {
        var harness = new BlazorHarness();
        harness.Services.AddAuthorizationCore();
        // BunitContext pre-registers its own placeholder IAuthorizationService
        // (which throws unless a test opts in) via AddSingleton, so
        // AddAuthorizationCore's TryAdd above cannot replace it. Register the
        // real ASP.NET Core implementation explicitly - the last registration
        // for a service type wins - to get real Roles evaluation instead.
        harness.Services.AddSingleton<IAuthorizationService, DefaultAuthorizationService>();
        harness.Services.AddCascadingAuthenticationState();
        harness.Services.AddSingleton<AuthenticationStateProvider>(new Ex028_TestAuthStateProvider(principal));
        return harness;
    }

    [Fact]
    public void Attack_Anonymous_Renders_Neither_Manager_Nor_Authenticated_Section()
    {
        using var harness = CreateHarness(Ex028_TestAuthStateProvider.Anonymous());

        var cut = harness.Render<Ex028_AuthorizeViewAndAuthState>();

        Assert.Empty(cut.FindAll("#manager-section"));
        Assert.Empty(cut.FindAll("#authenticated-section"));
    }

    [Fact]
    public void Attack_Authenticated_Without_Manager_Role_Does_Not_Render_Manager_Section()
    {
        using var harness = CreateHarness(Ex028_TestAuthStateProvider.AuthenticatedAs("ada"));

        var cut = harness.Render<Ex028_AuthorizeViewAndAuthState>();

        Assert.Empty(cut.FindAll("#manager-section"));
        Assert.Single(cut.FindAll("#authenticated-section"));
    }

    [Fact]
    public void Use_Manager_Role_Renders_Both_Sections()
    {
        using var harness = CreateHarness(Ex028_TestAuthStateProvider.AuthenticatedAs("ada", "manager"));

        var cut = harness.Render<Ex028_AuthorizeViewAndAuthState>();

        Assert.Single(cut.FindAll("#manager-section"));
        Assert.Single(cut.FindAll("#authenticated-section"));
    }

    [Fact]
    public void Use_Anonymous_Renders_NotAuthorized_Content_So_The_Page_Is_Not_Blank()
    {
        using var harness = CreateHarness(Ex028_TestAuthStateProvider.Anonymous());

        var cut = harness.Render<Ex028_AuthorizeViewAndAuthState>();

        Assert.Single(cut.FindAll("#anonymous-section"));
    }
}
