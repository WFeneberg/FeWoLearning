using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace FeWoLearning.Security.Exercises.Support;

// A fixed-state AuthenticationStateProvider for tests: constructed with the
// ClaimsPrincipal a test wants AuthorizeView to see, and it never changes
// after construction. Byte-identical in exercises/_support and
// solutions/_support - this file is never a catalog row.
public sealed class Ex028_TestAuthStateProvider : AuthenticationStateProvider
{
    private readonly AuthenticationState _state;

    public Ex028_TestAuthStateProvider(ClaimsPrincipal principal)
    {
        _state = new AuthenticationState(principal);
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(_state);

    public static ClaimsPrincipal Anonymous() => new(new ClaimsIdentity());

    public static ClaimsPrincipal AuthenticatedAs(string userName, params string[] roles)
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, userName) };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, authenticationType: "TestAuth");
        return new ClaimsPrincipal(identity);
    }
}
