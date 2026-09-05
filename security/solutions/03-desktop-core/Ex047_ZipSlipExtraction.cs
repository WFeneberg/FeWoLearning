using System.IO;
using System.IO.Compression;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 047 — ZipSlipExtraction (reference solution).
public static class Ex047_ZipSlipExtraction
{
    public static IReadOnlyList<string> ExtractTo(Stream archive, string destinationDirectory)
    {
        var destinationFullPath = Path.GetFullPath(destinationDirectory);
        var destinationRoot = destinationFullPath.EndsWith(Path.DirectorySeparatorChar)
            ? destinationFullPath
            : destinationFullPath + Path.DirectorySeparatorChar;

        Directory.CreateDirectory(destinationFullPath);

        var written = new List<string>();

        using var zip = new ZipArchive(archive, ZipArchiveMode.Read, leaveOpen: true);
        foreach (var entry in zip.Entries)
        {
            // A directory entry's Name (the final path segment) is empty; there is
            // nothing to write for it.
            if (string.IsNullOrEmpty(entry.Name)) continue;

            // Combine() ignores destinationFullPath outright when the entry name is
            // itself rooted (e.g. "C:\evil.txt"), which would otherwise let an
            // absolute entry escape immediately - reject it before that happens.
            if (Path.IsPathRooted(entry.FullName)) continue;

            var candidatePath = Path.GetFullPath(Path.Combine(destinationFullPath, entry.FullName));

            // GetFullPath collapses ".." segments, so an entry like
            // "../escaped.txt" or "sub/../../escaped.txt" resolves to its real,
            // final location here - and that location has to still be under the
            // destination root, checked with a trailing separator so a sibling
            // directory that merely shares the root's name as a prefix cannot
            // slip through.
            if (!candidatePath.StartsWith(destinationRoot, StringComparison.OrdinalIgnoreCase)) continue;

            var candidateDirectory = Path.GetDirectoryName(candidatePath);
            if (!string.IsNullOrEmpty(candidateDirectory)) Directory.CreateDirectory(candidateDirectory);

            using (var entryStream = entry.Open())
            using (var fileStream = File.Create(candidatePath))
            {
                entryStream.CopyTo(fileStream);
            }

            written.Add(candidatePath);
        }

        return written;
    }
}
