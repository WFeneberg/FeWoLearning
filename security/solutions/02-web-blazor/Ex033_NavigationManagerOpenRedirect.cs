using Microsoft.AspNetCore.Components;

namespace FeWoLearning.Security.Exercises.WebBlazor;

// Exercise 033 — NavigationManagerOpenRedirect (reference solution).
// Goal:   A component often forwards a caller-supplied "go here next" value
//         straight into NavigationManager.NavigateTo. GoTo is that forwarding
//         call, made safe: navigate to candidate only when it is a path
//         rooted at this application itself, otherwise navigate to "/".
// Drills: client-side redirect validation, external URI rejection.
// Passes: attack facts - "https://evil.example/", "//evil.example/"
//                        (protocol-relative), a "javascript:" URI, and null
//                        all navigate to this app's own "/" instead;
//         use facts     - "/dashboard" navigates to "/dashboard" unchanged,
//                        and "/reports?year=2026" navigates there with its
//                        query string preserved.
public static class Ex033_NavigationManagerOpenRedirect
{
    public static void GoTo(NavigationManager navigation, string? candidate)
    {
        navigation.NavigateTo(SafeLocalPath(candidate) ?? "/");
    }

    // Same shape as Ex022_OpenRedirectGuard (web-aspnet): reject anything
    // that is not rooted at exactly one leading slash - an absolute URL, a
    // "javascript:" scheme, or a protocol-relative "//host/..." - before
    // handing Uri a second opinion for whatever scheme this check did not
    // anticipate.
    private static string? SafeLocalPath(string? candidate)
    {
        if (string.IsNullOrEmpty(candidate))
        {
            return null;
        }

        var normalised = candidate.Replace('\\', '/');

        if (!normalised.StartsWith('/') || normalised.StartsWith("//"))
        {
            return null;
        }

        if (Uri.TryCreate(normalised, UriKind.Absolute, out _))
        {
            return null;
        }

        return candidate;
    }
}
