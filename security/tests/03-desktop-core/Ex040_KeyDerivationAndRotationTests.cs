using System.Security.Cryptography;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex040_KeyDerivationAndRotationTests
{
    private static byte[] NewMasterSecret()
    {
        var secret = new byte[32];
        RandomNumberGenerator.Fill(secret);
        return secret;
    }

    [Fact]
    public void Attack_A_Ring_That_Never_Rotated_Cannot_Decrypt_A_Post_Rotation_Envelope()
    {
        var masterSecret = NewMasterSecret();
        var rotated = new Ex040_KeyRing(masterSecret);
        var stale = new Ex040_KeyRing(masterSecret); // same master secret, never rotated

        rotated.Rotate();
        var envelope = rotated.Encrypt("after rotation"u8.ToArray());

        Assert.Throws<CryptographicException>(() => stale.Decrypt(envelope));
    }

    [Fact]
    public void Attack_Rings_From_Different_Master_Secrets_Cannot_Read_Each_Others_Envelopes_At_Any_Version()
    {
        var ringA = new Ex040_KeyRing(NewMasterSecret());
        var ringB = new Ex040_KeyRing(NewMasterSecret());

        var aV1 = ringA.Encrypt("A v1 payload"u8.ToArray());
        var bV1 = ringB.Encrypt("B v1 payload"u8.ToArray());

        ringA.Rotate();
        ringB.Rotate();

        var aV2 = ringA.Encrypt("A v2 payload"u8.ToArray());
        var bV2 = ringB.Encrypt("B v2 payload"u8.ToArray());

        Assert.Throws<AuthenticationTagMismatchException>(() => ringB.Decrypt(aV1));
        Assert.Throws<AuthenticationTagMismatchException>(() => ringB.Decrypt(aV2));
        Assert.Throws<AuthenticationTagMismatchException>(() => ringA.Decrypt(bV1));
        Assert.Throws<AuthenticationTagMismatchException>(() => ringA.Decrypt(bV2));
    }

    [Fact]
    public void Use_Data_Encrypted_Before_Rotation_Still_Decrypts_After_Rotation()
    {
        var ring = new Ex040_KeyRing(NewMasterSecret());
        var plaintext = "before rotation"u8.ToArray();
        var envelope = ring.Encrypt(plaintext);

        ring.Rotate();

        Assert.Equal(plaintext, ring.Decrypt(envelope));
    }

    [Fact]
    public void Use_CurrentVersion_Increments_By_Exactly_One_Per_Rotation()
    {
        var ring = new Ex040_KeyRing(NewMasterSecret());
        var start = ring.CurrentVersion;

        ring.Rotate();
        Assert.Equal(start + 1, ring.CurrentVersion);

        ring.Rotate();
        Assert.Equal(start + 2, ring.CurrentVersion);
    }

    [Fact]
    public void Use_Data_Encrypted_After_Rotation_Decrypts_Too()
    {
        var ring = new Ex040_KeyRing(NewMasterSecret());
        ring.Rotate();

        var plaintext = "after rotation"u8.ToArray();
        var envelope = ring.Encrypt(plaintext);

        Assert.Equal(plaintext, ring.Decrypt(envelope));
    }
}
