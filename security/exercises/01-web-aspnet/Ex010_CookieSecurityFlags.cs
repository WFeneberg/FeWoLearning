using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 010 — CookieSecurityFlags (web-aspnet).
// Goal:   Append a session cookie carrying the flags a session cookie needs -
//         HttpOnly so script cannot read it, Secure so it is never sent over
//         plain HTTP, SameSite=Strict so a cross-site request never carries it -
//         without breaking the cookie's own name, value or site-wide scope.
// Drills: HttpOnly, Secure, SameSite, cookie scope.
// Passes: attack facts   - the emitted Set-Cookie carries HttpOnly, Secure and
//                          SameSite=Strict (compared case-insensitively);
//         use facts      - the cookie's name and value round-trip exactly as
//                          given, including a value that needs URL encoding, and
//                          Path=/ is present so the cookie is actually usable.
public static class Ex010_CookieSecurityFlags
{
    public static void AppendSessionCookie(HttpResponse response, string name, string value) =>
        throw new NotImplementedException(
            "TODO: Ex010 - append a cookie with HttpOnly, Secure, SameSite=Strict and Path=/ set");
}
