using System.Security.Claims;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 017 — JwtValidation (web-aspnet).
// Goal:   Validate a JWT the way a resource server must: reject anything signed
//         with the wrong key, anything using the unsigned "none" algorithm,
//         anything from the wrong issuer or for the wrong audience, anything
//         expired, and anything whose payload was edited after signing - and
//         accept only a token that is correct on every one of those axes.
// Drills: issuer, audience, lifetime and signature validation, alg confusion.
// Passes: attack facts   - a token signed with a different key, an "alg: none"
//                          token, a token from a different issuer, a token for
//                          a different audience, an expired token, and a token
//                          whose payload was edited after signing all return
//                          false with a null principal;
//         use facts      - a correctly signed, in-date token for the right
//                          issuer and audience returns true and a principal
//                          carrying its sub claim.
public static class Ex017_JwtValidation
{
    public static bool TryValidate(
        string token, byte[] signingKey, string issuer, string audience, out ClaimsPrincipal? principal) =>
        throw new NotImplementedException(
            "TODO: Ex017 - validate signature, issuer, audience and lifetime with Microsoft.IdentityModel.JsonWebTokens, rejecting alg:none and tampered payloads");
}
