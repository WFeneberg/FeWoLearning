using Microsoft.AspNetCore.Builder;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 002 — HttpsRedirectAndHsts (web-aspnet).
// Goal:   Register middleware that redirects every plain-HTTP request to the
//         equivalent https URL - same path and query, on httpsPort - with a
//         permanent (308) redirect, and stamps Strict-Transport-Security on
//         responses that already arrived over TLS. Never the other way around:
//         a header promising "always use https" is worthless on a response that
//         itself proves the channel can still be downgraded.
// Drills: HSTS, transport downgrade, redirect status codes.
// Passes: attack facts   - a plain-HTTP GET receives a 308 whose Location is the
//                          same path and query on https; an https response
//                          carries Strict-Transport-Security with a max-age of at
//                          least one year and includeSubDomains;
//         use facts      - an https request is served directly, not redirected;
//                          a plain-HTTP response never carries
//                          Strict-Transport-Security.
public static class Ex002_HttpsRedirectAndHsts
{
    public static void Use(IApplicationBuilder app, int httpsPort) =>
        throw new NotImplementedException(
            "TODO: Ex002 - redirect plain-HTTP requests to https (preserving path/query) and add HSTS only to https responses");
}
