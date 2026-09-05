using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 009 — CorsPolicy (web-aspnet).
// Goal:   Register a CORS policy that allows exactly one caller-supplied origin
//         (never a wildcard) and apply it to the pipeline, answering preflight
//         requests directly, so a browser page served from any other origin gets
//         no cross-origin access at all.
// Drills: origin allowlists, credentials, why wildcard plus credentials fails.
// Passes: attack facts   - a request with Origin: https://evil.example receives
//                          no Access-Control-Allow-Origin header; the response
//                          never carries Access-Control-Allow-Origin: * together
//                          with Access-Control-Allow-Credentials: true;
//         use facts      - a request from the allowed origin receives
//                          Access-Control-Allow-Origin echoing exactly that
//                          origin; a preflight OPTIONS from the allowed origin
//                          returns 204 with the allowed methods.
public static class Ex009_CorsPolicy
{
    public static void AddServices(IServiceCollection services, string allowedOrigin) =>
        throw new NotImplementedException(
            "TODO: Ex009 - register a named CORS policy that allows only allowedOrigin");

    public static void Use(IApplicationBuilder app) =>
        throw new NotImplementedException("TODO: Ex009 - apply the named CORS policy to the pipeline");
}
