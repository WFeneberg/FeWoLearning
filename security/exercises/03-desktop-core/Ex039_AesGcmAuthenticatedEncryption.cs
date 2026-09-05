using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 039 — AesGcmAuthenticatedEncryption (desktop-core).
// Goal:   Build a self-contained "envelope" around AES-GCM: Encrypt packs a fresh
//         random nonce, the authentication tag and the ciphertext into one byte
//         array; Decrypt unpacks it and both decrypts and authenticates in one
//         step, so any tampering anywhere in the envelope is detected rather than
//         silently producing altered plaintext.
// Drills: AES-GCM, nonce uniqueness, tag verification, tamper detection.
// Passes: attack facts   - flipping any single byte anywhere in the envelope
//                          makes Decrypt throw CryptographicException, because the
//                          authentication tag no longer matches; truncating the
//                          envelope by one byte does too; decrypting with a
//                          different key does too; encrypting the same plaintext
//                          twice under the same key produces two envelopes whose
//                          nonce region differs, so an attacker watching many
//                          messages never sees a repeated (key, nonce) pair;
//         use facts      - Decrypt(k, Encrypt(k, p)) reproduces p for an empty, a
//                          small and a 1 MB plaintext.
public static class Ex039_AesGcmAuthenticatedEncryption
{
    public static byte[] Encrypt(byte[] key, byte[] plaintext) =>
        throw new NotImplementedException(
            "TODO: Ex039 - generate a fresh random nonce, run AesGcm.Encrypt, and pack nonce + tag + ciphertext into one byte array");

    public static byte[] Decrypt(byte[] key, byte[] envelope) =>
        throw new NotImplementedException(
            "TODO: Ex039 - split the envelope back into nonce + tag + ciphertext and run AesGcm.Decrypt");
}
