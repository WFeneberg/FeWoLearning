using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 043 — SignatureVerification (desktop-core).
// Goal:   Wrap ECDSA detached signing and verification so a caller can prove a
//         payload came from the holder of a specific private key and was not
//         altered afterwards. ECDsa.VerifyData is itself already hardened
//         against malformed, attacker-controlled signature bytes - however
//         garbled or however sized, verified experimentally across dozens of
//         malformed shapes (empty, truncated, oversized, all-zero, structurally
//         invalid) in both of .NET's signature formats, it returns false
//         rather than throwing. So Verify should be a thin, honest pass-
//         through with no defensive try/catch of its own: adding one here
//         would be dead code that implies a guard the platform already
//         provides for free, and dead code in a security exercise is worse
//         than no code at all.
// Drills: ECDSA sign/verify, detached signatures, tamper detection, and
//         recognising when a platform primitive already provides a safety
//         property so the caller does not need to reinvent it.
// Passes: attack facts   - flipping one byte of a signed payload makes Verify
//                          return false; a signature produced by a different
//                          key pair fails to verify; a battery of malformed
//                          signatures (empty, one byte, half-length all-zero,
//                          1000 bytes of garbage, 5000 bytes of garbage) all
//                          make Verify return false, never throw; truncating
//                          one byte off a genuine signature does too; feeding
//                          Verify a payload and signature swapped also returns
//                          false;
//         use facts      - Verify(p, Sign(p, priv), pub) is true for three
//                          payloads, including an empty one; signing the same
//                          payload twice and verifying both signatures
//                          succeeds for both (ECDSA signing is randomised, so
//                          the two signatures differ from each other - that
//                          is expected, not a bug).
public static class Ex043_SignatureVerification
{
    public static byte[] Sign(byte[] payload, ECDsa privateKey) =>
        throw new NotImplementedException(
            "TODO: Ex043 - sign payload with privateKey using SHA-256 (ECDsa.SignData)");

    public static bool Verify(byte[] payload, byte[] signature, ECDsa publicKey) =>
        throw new NotImplementedException(
            "TODO: Ex043 - verify signature over payload with publicKey using SHA-256 (ECDsa.VerifyData); no try/catch needed, VerifyData already returns false rather than throwing for a malformed signature");
}
