using Microsoft.AspNetCore.Components;

namespace FeWoLearning.Security.Exercises.WebBlazor;

// Exercise 033 — NavigationManagerOpenRedirect (web-blazor).
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
    public static void GoTo(NavigationManager navigation, string? candidate) =>
        throw new NotImplementedException(
            "TODO: Ex033 - navigation.NavigateTo(candidate) only when candidate is a path rooted at this app, otherwise navigation.NavigateTo(\"/\")");
}
