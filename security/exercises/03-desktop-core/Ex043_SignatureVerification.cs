using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 043 — SignatureVerification (desktop-core).
// Goal:   Wrap ECDSA detached signing and verification so a caller can prove a
//         payload came from the holder of a specific private key and was not
//         altered afterwards. Verify must never throw on attacker-controlled
//         input, however malformed - a signature check is exactly the kind of
//         code a hostile caller gets to poke at directly, and an unhandled
//         exception there is a denial-of-service waiting to happen.
// Drills: ECDSA sign/verify, detached signatures, tamper detection, rejecting
//         malformed signatures without throwing.
// Passes: attack facts   - flipping one byte of a signed payload makes Verify
//                          return false; a signature produced by a different
//                          key pair fails to verify; an empty signature and a
//                          truncated signature both make Verify return false
//                          without throwing; feeding Verify a payload and
//                          signature swapped - the signature bytes as the
//                          "payload" argument, the original payload bytes as
//                          the "signature" argument, chosen the same length -
//                          also returns false;
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
            "TODO: Ex043 - verify signature over payload with publicKey using SHA-256 (ECDsa.VerifyData), catching malformed-signature exceptions and returning false instead of throwing");
}
