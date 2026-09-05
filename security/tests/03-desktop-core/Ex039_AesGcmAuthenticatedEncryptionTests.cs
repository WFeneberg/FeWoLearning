using System.Security.Cryptography;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex039_AesGcmAuthenticatedEncryptionTests
{
    private const int NonceSize = 12;

    private static byte[] NewKey()
    {
        var key = new byte[32];
        RandomNumberGenerator.Fill(key);
        return key;
    }

    [Fact]
    public void Attack_Flipping_Any_Single_Envelope_Byte_Makes_Decrypt_Throw()
    {
        var key = NewKey();
        var envelope = Ex039_AesGcmAuthenticatedEncryption.Encrypt(key, "attack test message"u8.ToArray());

        for (var i = 0; i < envelope.Length; i++)
        {
            var tampered = (byte[])envelope.Clone();
            tampered[i] ^= 0xFF;

            Assert.Throws<AuthenticationTagMismatchException>(() => Ex039_AesGcmAuthenticatedEncryption.Decrypt(key, tampered));
        }
    }

    [Fact]
    public void Attack_Truncating_The_Envelope_By_One_Byte_Throws()
    {
        var key = NewKey();
        var envelope = Ex039_AesGcmAuthenticatedEncryption.Encrypt(key, "attack test message"u8.ToArray());

        var truncated = envelope[..^1];

        Assert.Throws<AuthenticationTagMismatchException>(() => Ex039_AesGcmAuthenticatedEncryption.Decrypt(key, truncated));
    }

    [Fact]
    public void Attack_Decrypting_With_A_Different_Key_Throws()
    {
        var key = NewKey();
        var wrongKey = NewKey();
        var envelope = Ex039_AesGcmAuthenticatedEncryption.Encrypt(key, "attack test message"u8.ToArray());

        Assert.Throws<AuthenticationTagMismatchException>(() => Ex039_AesGcmAuthenticatedEncryption.Decrypt(wrongKey, envelope));
    }

    [Fact]
    public void Attack_Encrypting_The_Same_Plaintext_Twice_Yields_Different_Nonces()
    {
        var key = NewKey();
        var plaintext = "same plaintext every time"u8.ToArray();

        var first = Ex039_AesGcmAuthenticatedEncryption.Encrypt(key, plaintext);
        var second = Ex039_AesGcmAuthenticatedEncryption.Encrypt(key, plaintext);

        var firstNonce = first[..NonceSize];
        var secondNonce = second[..NonceSize];

        Assert.False(firstNonce.SequenceEqual(secondNonce));
    }

    [Fact]
    public void Use_Round_Trip_Reproduces_An_Empty_Plaintext()
    {
        AssertRoundTrips(Array.Empty<byte>());
    }

    [Fact]
    public void Use_Round_Trip_Reproduces_A_Small_Plaintext()
    {
        AssertRoundTrips("a small secret message"u8.ToArray());
    }

    [Fact]
    public void Use_Round_Trip_Reproduces_A_1MB_Plaintext()
    {
        var plaintext = new byte[1024 * 1024];
        RandomNumberGenerator.Fill(plaintext);

        AssertRoundTrips(plaintext);
    }

    private static void AssertRoundTrips(byte[] plaintext)
    {
        var key = NewKey();

        var envelope = Ex039_AesGcmAuthenticatedEncryption.Encrypt(key, plaintext);
        var recovered = Ex039_AesGcmAuthenticatedEncryption.Decrypt(key, envelope);

        Assert.Equal(plaintext, recovered);
    }
}
