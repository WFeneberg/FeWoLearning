using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 003 — ContentSecurityPolicy (web-aspnet).
// Goal:   Register middleware that stamps a Content-Security-Policy header
//         locking scripts down to 'self' plus a fresh per-request nonce, denies
//         objects entirely, and never opens the unsafe-inline/unsafe-eval
//         escape hatches - then expose that same request's nonce through
//         GetNonce so a view can put it on the one inline <script> tag it needs.
// Drills: CSP directives, per-request nonce, inline-script blocking.
// Passes: attack facts   - the header always contains default-src 'self' and
//                          object-src 'none'; it never contains unsafe-inline or
//                          unsafe-eval; two separate requests receive different
//                          nonces;
//         use facts      - GetNonce returns the exact value that appears in that
//                          same request's script-src directive, and it decodes to
//                          at least 16 bytes.
public static class Ex003_ContentSecurityPolicy
{
    public static void Use(IApplicationBuilder app) =>
        throw new NotImplementedException(
            "TODO: Ex003 - add a CSP header with a fresh per-request nonce in script-src, and store the nonce so GetNonce can find it");

    public static string GetNonce(HttpContext context) =>
        throw new NotImplementedException(
            "TODO: Ex003 - return the nonce this request's Use middleware generated");
}
