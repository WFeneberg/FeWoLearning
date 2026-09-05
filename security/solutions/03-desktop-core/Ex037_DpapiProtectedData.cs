using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 037 — DpapiProtectedData (reference solution).
public static class Ex037_DpapiProtectedData
{
    public static byte[] Protect(byte[] plaintext, byte[] entropy) =>
        ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser);

    public static byte[] Unprotect(byte[] ciphertext, byte[] entropy) =>
        ProtectedData.Unprotect(ciphertext, entropy, DataProtectionScope.CurrentUser);
}
