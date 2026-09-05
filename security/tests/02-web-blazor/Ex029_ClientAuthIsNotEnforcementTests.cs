using System.Security.Claims;
using Bunit;
using FeWoLearning.Security.Exercises.Support;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex029_ClientAuthIsNotEnforcementTests
{
    // BlazorHarness already wires up real Roles/policy evaluation; the only
    // thing left to attach per test is which ClaimsPrincipal AuthorizeView
    // should see.
    private static BlazorHarness CreateHarness(ClaimsPrincipal principal)
    {
        var harness = new BlazorHarness();
        harness.Services.AddSingleton<AuthenticationStateProvider>(new Ex028_TestAuthStateProvider(principal));
        return harness;
    }

    [Fact]
    public void Attack_Direct_Call_With_A_Non_Approver_Principal_Returns_False()
    {
        var service = new Ex029_PayrollService();
        var caller = Ex028_TestAuthStateProvider.AuthenticatedAs("mallory");

        var approved = service.TryApprove(caller, requestId: 42, out var denial);

        Assert.False(approved);
        Assert.False(string.IsNullOrEmpty(denial));
    }

    [Fact]
    public void Attack_Direct_Call_With_An_Anonymous_Principal_Returns_False()
    {
        var service = new Ex029_PayrollService();
        var caller = Ex028_TestAuthStateProvider.Anonymous();

        var approved = service.TryApprove(caller, requestId: 42, out var denial);

        Assert.False(approved);
        Assert.False(string.IsNullOrEmpty(denial));
    }

    [Fact]
    public void Use_Direct_Call_With_An_Approver_Principal_Returns_True()
    {
        var service = new Ex029_PayrollService();
        var caller = Ex028_TestAuthStateProvider.AuthenticatedAs("ada", "approver");

        var approved = service.TryApprove(caller, requestId: 42, out var denial);

        Assert.True(approved);
        Assert.Null(denial);
    }

    [Fact]
    public void Use_Component_Renders_The_Approve_Button_For_An_Approver()
    {
        using var harness = CreateHarness(Ex028_TestAuthStateProvider.AuthenticatedAs("ada", "approver"));

        var cut = harness.Render<Ex029_ClientAuthIsNotEnforcement>();

        Assert.Single(cut.FindAll("#approve"));
    }

    [Fact]
    public void Use_Component_Hides_The_Approve_Button_For_A_Non_Approver()
    {
        using var harness = CreateHarness(Ex028_TestAuthStateProvider.AuthenticatedAs("mallory"));

        var cut = harness.Render<Ex029_ClientAuthIsNotEnforcement>();

        Assert.Empty(cut.FindAll("#approve"));
    }
}
