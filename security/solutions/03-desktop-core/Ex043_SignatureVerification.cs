using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 043 — SignatureVerification (reference solution).
public static class Ex043_SignatureVerification
{
    public static byte[] Sign(byte[] payload, ECDsa privateKey) =>
        privateKey.SignData(payload, HashAlgorithmName.SHA256);

    // No try/catch: ECDsa.VerifyData already returns false rather than
    // throwing for a malformed signature (verified experimentally against a
    // wide battery of malformed byte shapes, in both DSASignatureFormat
    // options). A try/catch here would be dead code.
    public static bool Verify(byte[] payload, byte[] signature, ECDsa publicKey) =>
        publicKey.VerifyData(payload, signature, HashAlgorithmName.SHA256);
}
