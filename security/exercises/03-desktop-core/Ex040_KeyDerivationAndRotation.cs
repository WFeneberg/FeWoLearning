using System.Buffers.Binary;
using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 040 — KeyDerivationAndRotation (desktop-core).
// Goal:   Build a small versioned key ring: it derives version 1's key from a
//         caller-supplied master secret, and each Rotate() derives the next
//         version's key from the *previous* version's key material (never
//         straight from the master secret again), remembering every version it
//         has ever walked through. Envelopes carry their version number, so
//         Encrypt always uses the current key and Decrypt looks up whichever
//         version produced the envelope it was handed.
// Drills: key derivation, versioned key material, decrypting older versions.
// Passes: attack facts   - an envelope produced by a ring after it has rotated
//                          cannot be decrypted by a second ring built from the
//                          same master secret that never rotated, because that
//                          ring's key material never walked forward to that
//                          version; two rings built from *different* master
//                          secrets can never read each other's envelopes, at any
//                          shared version number;
//         use facts      - after Rotate(), data encrypted before the rotation
//                          still decrypts (a ring must keep every version's key,
//                          not just the newest one); CurrentVersion increases by
//                          exactly one per Rotate() call; and data encrypted
//                          after the rotation decrypts too.
public sealed class Ex040_KeyRing
{
    public Ex040_KeyRing(byte[] masterSecret) =>
        throw new NotImplementedException(
            "TODO: Ex040 - derive version 1's key from masterSecret and remember it");

    public int CurrentVersion =>
        throw new NotImplementedException("TODO: Ex040 - track and expose the ring's current version");

    public void Rotate() =>
        throw new NotImplementedException(
            "TODO: Ex040 - derive the next version's key from the current version's key, bump CurrentVersion, and keep the old key too");

    public byte[] Encrypt(byte[] plaintext) =>
        throw new NotImplementedException(
            "TODO: Ex040 - encrypt with the current version's key (e.g. AES-GCM) and stamp the version number into the envelope");

    public byte[] Decrypt(byte[] envelope) =>
        throw new NotImplementedException(
            "TODO: Ex040 - read the version out of the envelope, look up that version's key, and decrypt with it");
}
