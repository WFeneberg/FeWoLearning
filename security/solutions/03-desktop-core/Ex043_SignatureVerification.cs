using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 043 — SignatureVerification (reference solution).
public static class Ex043_SignatureVerification
{
    public static byte[] Sign(byte[] payload, ECDsa privateKey) =>
        privateKey.SignData(payload, HashAlgorithmName.SHA256);

    public static bool Verify(byte[] payload, byte[] signature, ECDsa publicKey)
    {
        try
        {
            return publicKey.VerifyData(payload, signature, HashAlgorithmName.SHA256);
        }
        catch (CryptographicException)
        {
            // A malformed signature (wrong length, garbage bytes) is an
            // attacker-controlled input, not a program bug - reject it the
            // same way a well-formed-but-wrong signature is rejected.
            return false;
        }
    }
}
