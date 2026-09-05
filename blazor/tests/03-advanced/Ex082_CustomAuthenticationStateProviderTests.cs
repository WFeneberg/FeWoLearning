using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

// No AddAuthorization() here: the point of this row is the provider, so the real one
// goes into DI and CascadingAuthenticationState is left to consume it.
public class Ex082_CustomAuthenticationStateProviderTests : BunitContext
{
    private readonly Ex082_CustomAuthenticationStateProvider _provider = new();

    public Ex082_CustomAuthenticationStateProviderTests()
        => Services.AddSingleton<AuthenticationStateProvider>(_provider);

    [Fact]
    public async Task Starts_Anonymous()
    {
        var state = await _provider.GetAuthenticationStateAsync();

        Assert.False(state.User.Identity?.IsAuthenticated);
        Assert.Null(state.User.Identity?.Name);
    }

    // Ruling: a ClaimsIdentity built without an authentication type is not
    // authenticated, no matter how many claims it carries - so a provider that hands
    // back `new ClaimsIdentity([nameClaim])` reports the right name and still leaves
    // every AuthorizeView on the page showing the anonymous branch. Both halves are
    // asserted for that reason.
    [Fact]
    public async Task Signing_In_Produces_An_Actually_Authenticated_Principal()
    {
        _provider.SignIn("ada");

        var state = await _provider.GetAuthenticationStateAsync();

        Assert.Equal("ada", state.User.Identity?.Name);
        Assert.True(state.User.Identity?.IsAuthenticated);
        Assert.Equal(
            Ex082_CustomAuthenticationStateProvider.AuthenticationType,
            state.User.Identity?.AuthenticationType);
    }

    [Fact]
    public void The_Cascade_Starts_Anonymous_Too()
    {
        var cut = Render<Ex082_CustomAuthenticationStateProvider_Host>();

        Assert.Equal("anonymous", cut.Find("#user").TextContent);
        Assert.Equal("False", cut.Find("#authenticated").TextContent);
    }

    // Ruling: this is what NotifyAuthenticationStateChanged buys. Without the call
    // the provider's own state is correct and nothing on screen moves, because
    // CascadingAuthenticationState has no reason to re-supply its task - verified by
    // mutation.
    [Fact]
    public void Signing_In_Pushes_The_New_State_Through_The_Cascade()
    {
        var cut = Render<Ex082_CustomAuthenticationStateProvider_Host>();

        cut.InvokeAsync(() => _provider.SignIn("ada"));

        cut.WaitForAssertion(() => Assert.Equal("ada", cut.Find("#user").TextContent));
        Assert.Equal("True", cut.Find("#authenticated").TextContent);
    }

    [Fact]
    public void Signing_Out_Pushes_That_Through_As_Well()
    {
        var cut = Render<Ex082_CustomAuthenticationStateProvider_Host>();
        cut.InvokeAsync(() => _provider.SignIn("ada"));
        cut.WaitForAssertion(() => Assert.Equal("ada", cut.Find("#user").TextContent));

        cut.InvokeAsync(() => _provider.SignOut());

        cut.WaitForAssertion(() => Assert.Equal("anonymous", cut.Find("#user").TextContent));
        Assert.Equal("False", cut.Find("#authenticated").TextContent);
    }
}
