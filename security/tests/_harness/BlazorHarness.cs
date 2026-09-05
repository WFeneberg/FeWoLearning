using Bunit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Security.Tests.Harness;

// Thin wrapper over BunitContext so block-02 tests have one place to add the
// services the block needs (auth state, navigation, persistent component state).
//
// Note for every test file in this project: bUnit 2.9 still ships an obsolete
// Bunit.TestContext, which collides with xunit.v3's Xunit.TestContext. Any file
// that has `using Bunit;` and also touches TestContext fails CS0104. Add
// `using TestContext = Xunit.TestContext;` to those files.
public sealed class BlazorHarness : BunitContext
{
    public BlazorHarness()
    {
        // BunitContext pre-registers its own Bunit.TestDoubles.PlaceholderAuthorizationService
        // as IAuthorizationService, which throws MissingBunitAuthorizationException
        // the instant any AuthorizeView (or [Authorize]-gated component) evaluates
        // it. Services.AddAuthorizationCore() alone cannot displace that
        // registration: it registers via TryAdd, which is a no-op once a service
        // type already has an entry. Registering the real ASP.NET Core
        // implementation afterward works because the LAST registration for a
        // service type wins in the container - this is that fix, made once here
        // so every block-02 test inherits real Roles/policy evaluation instead
        // of every row rediscovering (or copy-pasting) it. AddCascadingAuthenticationState
        // supplies the Task<AuthenticationState> cascading parameter AuthorizeView
        // reads; a test that never renders an AuthorizeView never touches either
        // registration, so this is safe for every block-02 test, not only the
        // auth-flavoured ones. A test still registers its own
        // AuthenticationStateProvider (the ClaimsPrincipal it wants AuthorizeView
        // to see) - that part is necessarily per-test.
        Services.AddAuthorizationCore();
        Services.AddSingleton<IAuthorizationService, DefaultAuthorizationService>();
        Services.AddCascadingAuthenticationState();
    }
}
