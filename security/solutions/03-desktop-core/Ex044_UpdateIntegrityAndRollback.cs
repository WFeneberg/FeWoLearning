using System.Security.Cryptography;
using System.Text;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 044 — UpdateIntegrityAndRollback (reference solution).
public sealed record Ex044_UpdateManifest(string Version, string Sha256, byte[] Signature);

public static class Ex044_UpdateIntegrityAndRollback
{
    public static bool ShouldInstall(
        Ex044_UpdateManifest manifest,
        byte[] payload,
        ECDsa publisherKey,
        string installedVersion,
        out string? rejection)
    {
        var actualHash = Convert.ToHexString(SHA256.HashData(payload));
        if (!string.Equals(actualHash, manifest.Sha256, StringComparison.OrdinalIgnoreCase))
        {
            rejection = "The downloaded payload does not match the manifest's hash.";
            return false;
        }

        var signedData = Encoding.UTF8.GetBytes(manifest.Version + ":" + manifest.Sha256);
        if (!publisherKey.VerifyData(signedData, manifest.Signature, HashAlgorithmName.SHA256))
        {
            rejection = "The manifest's signature does not verify against the publisher key.";
            return false;
        }

        if (!Version.TryParse(manifest.Version, out var newVersion))
        {
            rejection = "The manifest's version could not be parsed.";
            return false;
        }

        if (!Version.TryParse(installedVersion, out var currentVersion))
        {
            rejection = "The installed version could not be parsed.";
            return false;
        }

        // Semantic, not textual: Version compares each dot-separated
        // component numerically, so "1.10.0" correctly outranks "1.9.0".
        if (newVersion.CompareTo(currentVersion) <= 0)
        {
            rejection = "The manifest's version is not newer than the installed version.";
            return false;
        }

        rejection = null;
        return true;
    }
}
