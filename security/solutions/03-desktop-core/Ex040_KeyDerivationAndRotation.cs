using System.Buffers.Binary;
using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 040 — KeyDerivationAndRotation (reference solution).
public sealed class Ex040_KeyRing
{
    private const int NonceSize = 12; // AesGcm.NonceByteSizes.MaxSize
    private const int TagSize = 16;   // AesGcm.TagByteSizes.MaxSize
    private const int VersionSize = sizeof(int);
    private const int KeySize = 32;   // AES-256

    private static readonly byte[] Info = "FeWoLearning.Ex040.KeyRing"u8.ToArray();

    private readonly Dictionary<int, byte[]> _keysByVersion = new();

    public Ex040_KeyRing(byte[] masterSecret)
    {
        CurrentVersion = 1;
        _keysByVersion[1] = DeriveKey(masterSecret, 1);
    }

    public int CurrentVersion { get; private set; }

    public void Rotate()
    {
        var previousKey = _keysByVersion[CurrentVersion];
        CurrentVersion++;
        _keysByVersion[CurrentVersion] = DeriveKey(previousKey, CurrentVersion);
    }

    public byte[] Encrypt(byte[] plaintext)
    {
        var version = CurrentVersion;
        var key = _keysByVersion[version];

        var nonce = new byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);
        var tag = new byte[TagSize];
        var ciphertext = new byte[plaintext.Length];

        using (var aes = new AesGcm(key, TagSize))
        {
            aes.Encrypt(nonce, plaintext, ciphertext, tag);
        }

        var envelope = new byte[VersionSize + NonceSize + TagSize + ciphertext.Length];
        BinaryPrimitives.WriteInt32BigEndian(envelope, version);
        Buffer.BlockCopy(nonce, 0, envelope, VersionSize, NonceSize);
        Buffer.BlockCopy(tag, 0, envelope, VersionSize + NonceSize, TagSize);
        Buffer.BlockCopy(ciphertext, 0, envelope, VersionSize + NonceSize + TagSize, ciphertext.Length);
        return envelope;
    }

    public byte[] Decrypt(byte[] envelope)
    {
        if (envelope.Length < VersionSize + NonceSize + TagSize)
            throw new CryptographicException("Envelope is too short to contain a version, nonce and tag.");

        var version = BinaryPrimitives.ReadInt32BigEndian(envelope.AsSpan(0, VersionSize));
        if (!_keysByVersion.TryGetValue(version, out var key))
            throw new CryptographicException($"This ring has no key material for version {version}.");

        var nonce = envelope.AsSpan(VersionSize, NonceSize);
        var tag = envelope.AsSpan(VersionSize + NonceSize, TagSize);
        var ciphertext = envelope.AsSpan(VersionSize + NonceSize + TagSize);

        var plaintext = new byte[ciphertext.Length];
        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);
        return plaintext;
    }

    private static byte[] DeriveKey(byte[] keyMaterial, int version)
    {
        var salt = new byte[VersionSize];
        BinaryPrimitives.WriteInt32BigEndian(salt, version);
        return HKDF.DeriveKey(HashAlgorithmName.SHA256, keyMaterial, KeySize, salt, Info);
    }
}
