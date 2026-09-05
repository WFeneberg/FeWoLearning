namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 018 — RefreshTokenRotation (web-aspnet).
// Goal:   Rotate refresh tokens on every redemption and detect reuse: a token
//         that gets redeemed a second time proves it was stolen (the
//         legitimate holder already moved on to its replacement), so the
//         whole family it belongs to - including whatever replacement chain
//         followed it - must stop working, without touching any other user's
//         tokens.
// Drills: single-use refresh tokens, reuse detection, family revocation.
// Passes: attack facts   - redeeming the same token twice fails the second
//                          time; after that reuse attempt, the replacement
//                          token issued from the reused token is also refused
//                          (family revocation); a token never issued is
//                          refused;
//         use facts      - a freshly issued token redeems once and yields a
//                          different replacement; the replacement itself
//                          redeems once; and a second user's tokens are
//                          unaffected by the first user's revocation.
public sealed class Ex018_RefreshTokenStore
{
    public string Issue(string userId) =>
        throw new NotImplementedException("TODO: Ex018 - issue a fresh single-use refresh token for userId");

    public bool TryRedeem(string refreshToken, out string? replacement) =>
        throw new NotImplementedException(
            "TODO: Ex018 - redeem a token exactly once and mint its replacement; reusing an already-redeemed token must revoke its whole family");
}
