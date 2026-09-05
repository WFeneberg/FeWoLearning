using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 010 — CookieSecurityFlags (reference solution).
public static class Ex010_CookieSecurityFlags
{
    public static void AppendSessionCookie(HttpResponse response, string name, string value) =>
        response.Cookies.Append(name, value, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
        });
}
