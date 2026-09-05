using Microsoft.AspNetCore.Builder;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 001 — SecurityHeaders (web-aspnet).
// Goal:   Register middleware that stamps three response headers on every response
//         - X-Content-Type-Options: nosniff, X-Frame-Options: DENY and
//         Referrer-Policy: no-referrer - without overwriting a value a downstream
//         component deliberately set for itself.
// Drills: middleware pipeline, Response.OnStarting, header lifetime.
// Passes: attack facts   - all three headers are present on a plain response, so a
//                          content-sniffing or clickjacking attack has nothing to
//                          work with;
//         use facts      - a handler that set its own Referrer-Policy keeps it, and
//                          the response body is delivered unchanged.
public static class Ex001_SecurityHeaders
{
    public static void Use(IApplicationBuilder app) =>
        throw new NotImplementedException(
            "TODO: Ex001 - add middleware that sets the three security headers without clobbering existing values");
}
