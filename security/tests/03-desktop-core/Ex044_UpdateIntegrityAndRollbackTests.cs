using System.Security.Cryptography;
using System.Text;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex044_UpdateIntegrityAndRollbackTests
{
    private static ECDsa NewKey() => ECDsa.Create(ECCurve.NamedCurves.nistP256);

    private static Ex044_UpdateManifest BuildManifest(byte[] payload, string version, ECDsa signingKey)
    {
        var hash = Convert.ToHexString(SHA256.HashData(payload));
        var signedData = Encoding.UTF8.GetBytes(version + ":" + hash);
        var signature = signingKey.SignData(signedData, HashAlgorithmName.SHA256);
        return new Ex044_UpdateManifest(version, hash, signature);
    }

    [Fact]
    public void Attack_Hash_Mismatch_Is_Rejected()
    {
        using var key = NewKey();
        var payload = "the real update payload"u8.ToArray();
        var manifest = BuildManifest(payload, "2.0.0", key);
        var wrongPayload = "a different payload entirely"u8.ToArray();

        var result = Ex044_UpdateIntegrityAndRollback.ShouldInstall(manifest, wrongPayload, key, "1.0.0", out var rejection);

        Assert.False(result);
        Assert.NotNull(rejection);
    }

    [Fact]
    public void Attack_Manifest_Signed_By_A_Different_Key_Is_Rejected()
    {
        using var publisherKey = NewKey();
        using var attackerKey = NewKey();
        var payload = "the real update payload"u8.ToArray();
        var manifest = BuildManifest(payload, "2.0.0", attackerKey);

        var result = Ex044_UpdateIntegrityAndRollback.ShouldInstall(manifest, payload, publisherKey, "1.0.0", out var rejection);

        Assert.False(result);
        Assert.NotNull(rejection);
    }

    [Fact]
    public void Attack_A_Lower_Version_Is_Rejected_Even_When_Perfectly_Signed()
    {
        using var key = NewKey();
        var payload = "the real update payload"u8.ToArray();
        var manifest = BuildManifest(payload, "1.0.0", key);

        var result = Ex044_UpdateIntegrityAndRollback.ShouldInstall(manifest, payload, key, "2.0.0", out var rejection);

        Assert.False(result);
        Assert.NotNull(rejection);
    }

    [Fact]
    public void Attack_An_Equal_Version_Is_Rejected()
    {
        using var key = NewKey();
        var payload = "the real update payload"u8.ToArray();
        var manifest = BuildManifest(payload, "1.5.0", key);

        var result = Ex044_UpdateIntegrityAndRollback.ShouldInstall(manifest, payload, key, "1.5.0", out var rejection);

        Assert.False(result);
        Assert.NotNull(rejection);
    }

    [Fact]
    public void Use_A_Correctly_Signed_Correctly_Hashed_Higher_Version_Installs()
    {
        using var key = NewKey();
        var payload = "the real update payload"u8.ToArray();
        var manifest = BuildManifest(payload, "2.0.0", key);

        var result = Ex044_UpdateIntegrityAndRollback.ShouldInstall(manifest, payload, key, "1.0.0", out var rejection);

        Assert.True(result);
        Assert.Null(rejection);
    }

    [Fact]
    public void Use_Version_Comparison_Is_Semantic_So_1_10_0_Beats_1_9_0()
    {
        using var key = NewKey();
        var payload = "the real update payload"u8.ToArray();
        var manifest = BuildManifest(payload, "1.10.0", key);

        var result = Ex044_UpdateIntegrityAndRollback.ShouldInstall(manifest, payload, key, "1.9.0", out var rejection);

        Assert.True(result);
        Assert.Null(rejection);
    }
}
