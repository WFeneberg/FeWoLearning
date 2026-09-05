using System.Security.Cryptography;
using System.Text;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 041 — FixedTimeComparison (reference solution).
public static class Ex041_FixedTimeComparison
{
    public static bool TokensMatch(string presented, string expected)
    {
        // Hashing first collapses both inputs to the same fixed length (32 bytes)
        // regardless of how long or short the original strings were, so the
        // comparison that follows never has a length-dependent shape to exploit.
        var presentedDigest = SHA256.HashData(Encoding.UTF8.GetBytes(presented));
        var expectedDigest = SHA256.HashData(Encoding.UTF8.GetBytes(expected));

        return CryptographicOperations.FixedTimeEquals(presentedDigest, expectedDigest);
    }
}
