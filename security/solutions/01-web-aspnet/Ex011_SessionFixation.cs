using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 011 — SessionFixation (reference solution).
public static class Ex011_SessionFixation
{
    private const string CookieName = "sid";

    private static readonly ConcurrentDictionary<string, string?> Sessions = new();

    public static string SignIn(HttpContext context, string userName)
    {
        if (context.Request.Cookies.TryGetValue(CookieName, out var oldId) && oldId is not null)
        {
            // Session-fixation defense: whatever identifier the attacker fixed in
            // advance must never be promoted to the authenticated user - it
            // collapses back to anonymous instead of carrying on.
            Sessions[oldId] = null;
        }

        var newId = Guid.NewGuid().ToString("N");
        Sessions[newId] = userName;
        context.Response.Cookies.Append(CookieName, newId, new CookieOptions { HttpOnly = true, Path = "/" });
        return newId;
    }

    public static string? Resolve(string sessionId) =>
        Sessions.TryGetValue(sessionId, out var userName) ? userName : null;
}
