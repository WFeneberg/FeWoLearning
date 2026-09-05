using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 008 — AntiforgeryCsrf (web-aspnet).
// Goal:   Register antiforgery services and middleware that validates the paired
//         request-token/cookie on every state-changing request, while never
//         challenging a safe method - a GET that required a CSRF token would
//         break every plain link on the site.
// Drills: antiforgery tokens, cross-origin POST, safe vs unsafe methods.
// Passes: attack facts   - a POST with no antiforgery token is answered 400; a
//                          POST carrying a token but not its matching cookie is
//                          answered 400;
//         use facts      - a GET is never challenged (200); a POST carrying both
//                          the token and its matching cookie succeeds (200) and
//                          the handler observed the request body.
public static class Ex008_AntiforgeryCsrf
{
    public static void AddServices(IServiceCollection services) =>
        throw new NotImplementedException("TODO: Ex008 - register antiforgery services");

    public static void Use(IApplicationBuilder app) =>
        throw new NotImplementedException(
            "TODO: Ex008 - validate the antiforgery token/cookie pair on unsafe methods only, answering 400 on failure");
}
