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

    // TODO: hand back the current state - an AuthenticationState wrapping the
    // ClaimsPrincipal for whoever is signed in, or an anonymous one when nobody is.
    // This is called by every consumer of the cascade, so it must be cheap and must
    // not itself sign anyone in.
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => throw new NotImplementedException("TODO: Ex082 - report the current state");

    // TODO: sign the named user in, then tell the framework.
    //   - Build a ClaimsPrincipal carrying a ClaimTypes.Name claim for `name`.
    //     Watch the identity: a ClaimsIdentity built without an authentication type
    //     is NOT authenticated, however many claims you give it - so AuthorizeView
    //     and friends would still treat the user as anonymous. Use AuthenticationType
    //     above.
    //   - Call NotifyAuthenticationStateChanged with a task for the new state.
    //     Without it nothing re-reads the state, and the UI keeps showing the old
    //     user until something else happens to re-render it.
    public void SignIn(string name)
        => throw new NotImplementedException("TODO: Ex082 - sign in and notify");

    // TODO: back to anonymous, and notify again.
    public void SignOut()
        => throw new NotImplementedException("TODO: Ex082 - sign out and notify");
}
