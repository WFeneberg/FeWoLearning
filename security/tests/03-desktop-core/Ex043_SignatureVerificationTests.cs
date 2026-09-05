using System.Security.Cryptography;
using System.Text;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex043_SignatureVerificationTests
{
    private static ECDsa NewKey() => ECDsa.Create(ECCurve.NamedCurves.nistP256);

    [Fact]
    public void Attack_Flipping_One_Payload_Byte_Fails_Verification()
    {
        using var key = NewKey();
        var payload = "attack payload"u8.ToArray();
        var signature = Ex043_SignatureVerification.Sign(payload, key);

        var tampered = (byte[])payload.Clone();
        tampered[0] ^= 0xFF;

        Assert.False(Ex043_SignatureVerification.Verify(tampered, signature, key));
    }

    [Fact]
    public void Attack_Signature_From_A_Different_Key_Pair_Fails_Verification()
    {
        using var signingKey = NewKey();
        using var otherKey = NewKey();
        var payload = "attack payload"u8.ToArray();
        var signature = Ex043_SignatureVerification.Sign(payload, signingKey);

        Assert.False(Ex043_SignatureVerification.Verify(payload, signature, otherKey));
    }

    [Fact]
    public void Attack_Empty_Signature_Fails_Verification_Without_Throwing()
    {
        using var key = NewKey();
        var payload = "attack payload"u8.ToArray();

        Assert.False(Ex043_SignatureVerification.Verify(payload, Array.Empty<byte>(), key));
    }

    [Fact]
    public void Attack_Truncated_Signature_Fails_Verification_Without_Throwing()
    {
        using var key = NewKey();
        var payload = "attack payload"u8.ToArray();
        var signature = Ex043_SignatureVerification.Sign(payload, key);

        Assert.False(Ex043_SignatureVerification.Verify(payload, signature[..^1], key));
    }

    [Fact]
    public void Attack_Swapping_Payload_And_Signature_Fails_Verification()
    {
        using var key = NewKey();
        // A P-256 IeeeP1363 signature is a fixed 64 bytes, so a 64-byte
        // payload lets the two arguments swap places without tripping a
        // length check - the swap must still be rejected on its own merits.
        var payload = new byte[64];
        RandomNumberGenerator.Fill(payload);
        var signature = Ex043_SignatureVerification.Sign(payload, key);
        Assert.Equal(payload.Length, signature.Length);

        Assert.False(Ex043_SignatureVerification.Verify(signature, payload, key));
    }

    [Theory]
    [InlineData("")]
    [InlineData("a short message")]
    [InlineData("a rather longer message used to exercise signature verification across a payload that spans more than one hash block, several times over")]
    public void Use_Verify_Accepts_A_Signature_Produced_By_Sign(string text)
    {
        using var key = NewKey();
        var payload = Encoding.UTF8.GetBytes(text);

        var signature = Ex043_SignatureVerification.Sign(payload, key);

        Assert.True(Ex043_SignatureVerification.Verify(payload, signature, key));
    }

    [Fact]
    public void Use_Signing_The_Same_Payload_Twice_Both_Verify()
    {
        using var key = NewKey();
        var payload = "same payload every time"u8.ToArray();

        var first = Ex043_SignatureVerification.Sign(payload, key);
        var second = Ex043_SignatureVerification.Sign(payload, key);

        Assert.True(Ex043_SignatureVerification.Verify(payload, first, key));
        Assert.True(Ex043_SignatureVerification.Verify(payload, second, key));
        // ECDSA signing draws a fresh random nonce every time, so two
        // signatures over the same payload are expected to differ.
        Assert.False(first.SequenceEqual(second));
    }
}
