using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 017 — JwtValidation (reference solution).
public static class Ex017_JwtValidation
{
    public static bool TryValidate(
        string token, byte[] signingKey, string issuer, string audience, out ClaimsPrincipal? principal)
    {
        var handler = new JsonWebTokenHandler();
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKey),
            RequireSignedTokens = true,
            // Pin the accepted algorithm explicitly - RequireSignedTokens alone
            // already rejects an unsigned "alg: none" token, but a named
            // allowlist is what stops a *different* signed-but-weaker algorithm
            // from sneaking through if this ever grows asymmetric keys too.
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };

        // Never throws on a validation failure - the outcome lives in the
        // result, which is exactly the shape TryValidate needs to expose.
        var result = handler.ValidateTokenAsync(token, parameters).GetAwaiter().GetResult();

        if (!result.IsValid || result.ClaimsIdentity is null)
        {
            principal = null;
            return false;
        }

        principal = new ClaimsPrincipal(result.ClaimsIdentity);
        return true;
    }
}
