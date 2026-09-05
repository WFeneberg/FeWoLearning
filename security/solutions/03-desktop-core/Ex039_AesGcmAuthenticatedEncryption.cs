using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 039 — AesGcmAuthenticatedEncryption (reference solution).
public static class Ex039_AesGcmAuthenticatedEncryption
{
    private const int NonceSize = 12; // AesGcm.NonceByteSizes.MaxSize
    private const int TagSize = 16;   // AesGcm.TagByteSizes.MaxSize

    public static byte[] Encrypt(byte[] key, byte[] plaintext)
    {
        var nonce = new byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);

        var tag = new byte[TagSize];
        var ciphertext = new byte[plaintext.Length];

        using (var aes = new AesGcm(key, TagSize))
        {
            aes.Encrypt(nonce, plaintext, ciphertext, tag);
        }

        var envelope = new byte[NonceSize + TagSize + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, envelope, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, envelope, NonceSize, TagSize);
        Buffer.BlockCopy(ciphertext, 0, envelope, NonceSize + TagSize, ciphertext.Length);
        return envelope;
    }

    public static byte[] Decrypt(byte[] key, byte[] envelope)
    {
        if (envelope.Length < NonceSize + TagSize)
            throw new CryptographicException("Envelope is too short to contain a nonce and a tag.");

        var nonce = envelope.AsSpan(0, NonceSize);
        var tag = envelope.AsSpan(NonceSize, TagSize);
        var ciphertext = envelope.AsSpan(NonceSize + TagSize);

        var plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);
        return plaintext;
    }
}
