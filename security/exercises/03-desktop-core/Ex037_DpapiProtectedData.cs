using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 037 — DpapiProtectedData (desktop-core).
// Goal:   Wrap the Windows Data Protection API so a caller can protect a byte
//         payload at rest under the current user's profile, with an optional
//         extra "entropy" value the caller must also supply to unprotect it -
//         and get a real CryptographicException back, not the plaintext, when
//         that entropy does not match.
// Drills: ProtectedData, DataProtectionScope, optional entropy.
// Passes: attack facts   - the protected bytes never contain a non-empty,
//                          distinctive plaintext as a contiguous subsequence, so
//                          DPAPI is not merely relabelling the input; unprotecting
//                          with the wrong entropy throws CryptographicException
//                          rather than silently returning something wrong or
//                          right; protecting the same plaintext twice produces two
//                          different byte sequences, because DPAPI folds in its
//                          own randomness rather than being a deterministic map;
//         use facts      - Unprotect(Protect(p, e), e) reproduces p exactly for
//                          several inputs, including an empty array, and a 1 MB
//                          payload round-trips too.
public static class Ex037_DpapiProtectedData
{
    public static byte[] Protect(byte[] plaintext, byte[] entropy) =>
        throw new NotImplementedException(
            "TODO: Ex037 - call ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser)");

    public static byte[] Unprotect(byte[] ciphertext, byte[] entropy) =>
        throw new NotImplementedException(
            "TODO: Ex037 - call ProtectedData.Unprotect(ciphertext, entropy, DataProtectionScope.CurrentUser)");
}
