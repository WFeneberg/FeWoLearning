using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace FeWoLearning.Blazor.Exercises.Advanced;

/// <summary>
/// Exercise 082 - Custom Authentication State Provider (advanced).
/// Goal:   Be the source of truth for who is signed in, and tell the framework when
///         that changes so everything cascading off it re-renders.
/// Drills: overriding GetAuthenticationStateAsync, building a ClaimsPrincipal that
///         actually counts as authenticated, and NotifyAuthenticationStateChanged.
/// Passes: dotnet test --filter FullyQualifiedName~Ex082_
/// </summary>
public sealed class Ex082_CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    public const string AuthenticationType = "FeWoLearning";

    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    private ClaimsPrincipal _current = Anonymous;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(_current));

    public void SignIn(string name)
    {
        // The authentication type is what makes the identity authenticated; without
        // it this principal carries the name and still counts as anonymous.
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, name)], AuthenticationType);
        _current = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void SignOut()
    {
        _current = Anonymous;

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
