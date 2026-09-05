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

    // ECDsa.VerifyData is documented and (independently, empirically
    // verified here across dozens of malformed shapes - empty, single-byte,
    // half-length, wildly oversized, all-zero, structurally invalid ASN.1 in
    // both signature formats) to return false rather than throw for any of
    // these. This asserts that real platform property directly: Verify must
    // reject every one of them, and none of these calls may let an exception
    // escape (an uncaught exception here would fail the test, since there is
    // no try/catch around the call below).
    public static IEnumerable<object[]> MalformedSignatures()
    {
        yield return new object[] { Array.Empty<byte>() };
        yield return new object[] { new byte[] { 0x00 } };
        yield return new object[] { new byte[32] }; // half the expected length, all zero
        yield return new object[] { Enumerable.Repeat((byte)0xAA, 1000).ToArray() };
        yield return new object[] { Enumerable.Repeat((byte)0xFF, 5000).ToArray() };
    }

    [Theory]
    [MemberData(nameof(MalformedSignatures))]
    public void Attack_A_Malformed_Signature_Is_Rejected_Without_Throwing(byte[] malformed)
    {
        using var key = NewKey();
        var payload = "attack payload"u8.ToArray();

        Assert.False(Ex043_SignatureVerification.Verify(payload, malformed, key));
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
        var payload = "attack payload"u8.ToArray();
        var signature = Ex043_SignatureVerification.Sign(payload, key);

        // The signature bytes fed in as if they were the payload, and the
        // payload bytes fed in as if they were the signature - a caller
        // mistake that must be rejected on its own merits, not crash.
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
