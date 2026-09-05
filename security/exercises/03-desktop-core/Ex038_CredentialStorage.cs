using System.IO;
using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 038 — CredentialStorage (desktop-core).
// Goal:   Build a small on-disk credential store: given a directory, Save writes
//         a named secret somewhere under it and Load reads it back - but nothing
//         written to disk may ever be the secret's own plaintext bytes.
// Drills: never plaintext at rest, round-tripping, scope of protection.
// Passes: attack facts   - after Save, no file anywhere under the directory
//                          contains the secret as a contiguous run of bytes (a
//                          store that "protects" by merely copying the bytes
//                          elsewhere fails this); Load for a name that was never
//                          saved returns null rather than throwing;
//         use facts      - Load returns exactly what Save stored, for a secret
//                          containing non-ASCII text and for a 4 KB secret; and
//                          saving a second value under a name already used
//                          overwrites it rather than appending, so Load returns
//                          only the newest value.
public sealed class Ex038_CredentialStore
{
    public Ex038_CredentialStore(string directory) =>
        throw new NotImplementedException(
            "TODO: Ex038 - remember the directory (creating it if it does not exist yet)");

    public void Save(string name, string secret) =>
        throw new NotImplementedException(
            "TODO: Ex038 - encrypt the secret (e.g. via DPAPI) and write it to a file derived from `name`, overwriting any previous value");

    public string? Load(string name) =>
        throw new NotImplementedException(
            "TODO: Ex038 - return null when `name` was never saved, otherwise decrypt and return the stored secret");
}
