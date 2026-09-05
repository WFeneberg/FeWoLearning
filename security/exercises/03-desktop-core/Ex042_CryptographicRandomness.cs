using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 042 — CryptographicRandomness (desktop-core).
// Goal:   Generate opaque tokens (session ids, password-reset codes, API keys)
//         from a cryptographically secure source instead of System.Random,
//         and encode them so they drop straight into a URL or a cookie value
//         with no escaping required.
// Drills: RandomNumberGenerator over System.Random, token generation, URL-safe
//         encoding.
// Passes: attack facts   - 1000 generated tokens never collide; none of 1000
//                          tokens equals what a new Random(seed)-driven
//                          generator would have produced, for any seed from 0
//                          to 999 - System.Random is deterministic from its
//                          seed, so a token generator built on it is
//                          reproducible by anyone who can guess or brute-force
//                          that seed, which RandomNumberGenerator is not;
//                          NewToken(0) throws ArgumentOutOfRangeException,
//                          because a zero-length token is never a caller's
//                          real intent and should fail loudly rather than
//                          silently returning an empty string;
//         use facts      - NewToken(32) decodes back to exactly 32 bytes; the
//                          encoding never contains '+', '/' or '=', so the
//                          token is safe to embed in a URL or cookie as-is.
public static class Ex042_CryptographicRandomness
{
    public static string NewToken(int byteCount) =>
        throw new NotImplementedException(
            "TODO: Ex042 - reject byteCount <= 0, fill byteCount bytes with RandomNumberGenerator, and encode them as unpadded URL-safe base64 (no '+', '/' or '=')");
}
