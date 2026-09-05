using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 011 — SessionFixation (web-aspnet).
// Goal:   Sign a user in by minting a brand-new session identifier instead of
//         promoting whatever identifier the incoming request already carried -
//         an attacker who fixes a victim's session identifier before login must
//         never be able to reuse it to hijack the session afterwards.
// Drills: identifier regeneration on privilege change.
// Passes: attack facts   - the identifier SignIn returns is never the one the
//                          request presented beforehand, and presenting that
//                          old identifier afterwards resolves to an anonymous
//                          session, never to the signed-in user;
//         use facts      - the identifier SignIn returns resolves to a session
//                          carrying userName, and presenting it twice in a row
//                          resolves to the same session both times.
public static class Ex011_SessionFixation
{
    public static string SignIn(HttpContext context, string userName) =>
        throw new NotImplementedException(
            "TODO: Ex011 - mint a fresh session identifier bound to userName, set it on the response, and make sure the request's pre-existing identifier (if any) resolves to an anonymous session afterwards");

    public static string? Resolve(string sessionId) =>
        throw new NotImplementedException(
            "TODO: Ex011 - return the user name bound to sessionId, or null when the identifier is unknown or anonymous");
}
