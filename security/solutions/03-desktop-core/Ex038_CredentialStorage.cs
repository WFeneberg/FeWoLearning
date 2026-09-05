using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 038 — CredentialStorage (reference solution).
public sealed class Ex038_CredentialStore
{
    // Fixed application-level entropy, not a secret: it only scopes this store's
    // ciphertexts away from any other DPAPI consumer sharing the same user
    // profile. The actual confidentiality guarantee comes from DPAPI's
    // per-user master key, not from this value.
    private static readonly byte[] Entropy = "FeWoLearning.Ex038.CredentialStore.v1"u8.ToArray();

    private readonly string _directory;

    public Ex038_CredentialStore(string directory)
    {
        _directory = directory;
        Directory.CreateDirectory(_directory);
    }

    public void Save(string name, string secret)
    {
        var plaintext = Encoding.UTF8.GetBytes(secret);
        var protectedBytes = ProtectedData.Protect(plaintext, Entropy, DataProtectionScope.CurrentUser);

        // WriteAllBytes truncates an existing file, so a second Save under the
        // same name overwrites rather than appending.
        File.WriteAllBytes(PathFor(name), protectedBytes);
    }

    public string? Load(string name)
    {
        var path = PathFor(name);
        if (!File.Exists(path)) return null;

        var protectedBytes = File.ReadAllBytes(path);
        var plaintext = ProtectedData.Unprotect(protectedBytes, Entropy, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(plaintext);
    }

    // Hashing the name keeps arbitrary caller-supplied names (including ones with
    // path separators) from ever reaching the filesystem as a path segment.
    private string PathFor(string name)
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(name));
        return Path.Combine(_directory, Convert.ToHexString(digest) + ".cred");
    }
}
