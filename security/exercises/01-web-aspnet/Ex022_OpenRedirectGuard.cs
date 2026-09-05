namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 022 — OpenRedirectGuard (web-aspnet).
// Goal:   Given a caller-supplied "return to this page after login" candidate,
//         return it unchanged only when it is a path rooted at this application
//         itself - otherwise return the fallback, so a login flow can never be
//         made to bounce a victim on to an attacker's site.
// Drills: local-redirect checks, absolute URL rejection, return-URL allowlists.
// Passes: attack facts   - "https://evil.example/", "//evil.example/" (protocol-
//                          relative), "/\evil.example" and "http:/\/\evil.example"
//                          (backslash variants a browser still treats as
//                          protocol-relative), a "javascript:" URL, and null all
//                          return fallback;
//         use facts      - "/dashboard" returns "/dashboard" unchanged, and
//                          "/reports?year=2026" returns unchanged including the
//                          query string.
public static class Ex022_OpenRedirectGuard
{
    public static string SafeReturnUrl(string? candidate, string fallback) =>
        throw new NotImplementedException(
            "TODO: Ex022 - return candidate unchanged only when it is a path rooted at this app, otherwise return fallback");
}
