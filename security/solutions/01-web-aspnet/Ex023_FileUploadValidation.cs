using System.IO;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 023 — FileUploadValidation (reference solution).
public static class Ex023_FileUploadValidation
{
    // Keyed by the only extensions this exercise ever accepts. Each value is the
    // magic-byte prefix that extension's real file format begins with - the
    // check that catches "report.pdf" actually being an .exe in disguise.
    private static readonly Dictionary<string, byte[]> MagicBytesByExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        [".png"] = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A],
        [".jpg"] = [0xFF, 0xD8, 0xFF],
        [".jpeg"] = [0xFF, 0xD8, 0xFF],
        [".gif"] = [0x47, 0x49, 0x46, 0x38],
        [".pdf"] = [0x25, 0x50, 0x44, 0x46],
    };

    public static bool TryAccept(string clientFileName, byte[] content, long maxBytes, out string storageName, out string? rejection)
    {
        storageName = "";

        // Path.GetExtension only ever looks at text after the final '.' - a
        // "../../evil.png" still yields ".png" here, and nothing about the rest
        // of clientFileName is read again below, so no path segment from the
        // caller can reach storageName at all.
        var extension = Path.GetExtension(clientFileName).ToLowerInvariant();

        if (!MagicBytesByExtension.TryGetValue(extension, out var magic))
        {
            rejection = "file extension is not allowed";
            return false;
        }

        if (content.Length > maxBytes)
        {
            rejection = "file exceeds the maximum allowed size";
            return false;
        }

        // The extension is a claim the client made about itself; this is the
        // evidence. A ".pdf" whose first bytes are "MZ" (an executable) fails
        // here regardless of what its name says.
        if (content.Length < magic.Length || !content.AsSpan(0, magic.Length).SequenceEqual(magic))
        {
            rejection = "file content does not match its declared extension";
            return false;
        }

        rejection = null;
        // Guid.NewGuid, not any transform of clientFileName: the storage name is
        // built from nothing the caller supplied except the (now-validated)
        // extension, so it is both unpredictable and immune to path injection.
        storageName = Guid.NewGuid().ToString("N") + extension;
        return true;
    }
}
