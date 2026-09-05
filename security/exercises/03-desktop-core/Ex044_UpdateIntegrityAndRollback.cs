using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 044 — UpdateIntegrityAndRollback (desktop-core).
// Goal:   Decide whether an auto-updater should install a downloaded update,
//         given a manifest describing it. Installing must require all three:
//         the payload actually hashes to what the manifest claims, the
//         manifest itself is signed by the publisher's key, and the
//         manifest's version is genuinely newer than what is already
//         installed - an attacker who can replay an old, still-validly-signed
//         manifest must not be able to downgrade a victim onto a version with
//         a known vulnerability.
// Drills: hash manifests, signed manifests, monotonic version enforcement
//         (rollback protection), semantic version comparison.
// Passes: attack facts   - a manifest whose Sha256 does not match the actual
//                          payload is rejected; a manifest signed by a
//                          different key than the one being checked against is
//                          rejected; a manifest whose Version is lower than
//                          installedVersion is rejected even though its hash
//                          and signature are both perfectly valid (rollback);
//                          a manifest whose Version exactly equals
//                          installedVersion is rejected too (installing it
//                          again is never the point of an update);
//         use facts      - a manifest that is correctly hashed, correctly
//                          signed, and genuinely newer than installedVersion
//                          returns true with rejection null; version
//                          comparison is semantic (parsed, not textual), so
//                          "1.10.0" is correctly accepted as newer than
//                          "1.9.0" - a plain string comparison would get that
//                          backwards.
public sealed record Ex044_UpdateManifest(string Version, string Sha256, byte[] Signature);

public static class Ex044_UpdateIntegrityAndRollback
{
    public static bool ShouldInstall(
        Ex044_UpdateManifest manifest,
        byte[] payload,
        ECDsa publisherKey,
        string installedVersion,
        out string? rejection) =>
        throw new NotImplementedException(
            "TODO: Ex044 - check payload's SHA-256 against manifest.Sha256, verify manifest.Signature over (Version + Sha256) with publisherKey, then require manifest.Version > installedVersion using System.Version, not string comparison");
}
