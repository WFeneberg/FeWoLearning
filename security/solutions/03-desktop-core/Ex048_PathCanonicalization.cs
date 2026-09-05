using System.IO;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 048 — PathCanonicalization (reference solution).
public static class Ex048_PathCanonicalization
{
    public static bool IsInside(string root, string candidate)
    {
        // Device paths (\\?\C:\...) tell the Win32 layer to skip all normalization,
        // including collapsing "..", so GetFullPath cannot be trusted to reason
        // about them the normal way - and UNC paths (\\server\share\...) name an
        // entirely different namespace than a local drive root. Neither is
        // something this method can safely certify as "inside" a local root, so
        // both are rejected outright.
        if (candidate.StartsWith(@"\\", StringComparison.Ordinal)) return false;

        // An NTFS alternate-data-stream suffix (":streamname" tacked onto a file
        // path) names a hidden secondary stream on that file, not the file's own
        // content - a colon anywhere but the drive-letter position (index 1, as in
        // "C:") signals exactly that.
        for (var i = 0; i < candidate.Length; i++)
        {
            var isDriveColon = i == 1 && candidate.Length > 1 && candidate[1] == ':';
            if (candidate[i] == ':' && !isDriveColon) return false;
        }

        string rootFullPath, candidateFullPath;
        try
        {
            rootFullPath = Path.GetFullPath(root);
            candidateFullPath = Path.GetFullPath(candidate);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return false;
        }

        var rootWithSeparator = rootFullPath.EndsWith(Path.DirectorySeparatorChar)
            ? rootFullPath
            : rootFullPath + Path.DirectorySeparatorChar;

        // Comparing against rootWithSeparator (not the bare root) is what tells
        // "C:\data-evil\file.txt" apart from "C:\data\file.txt": without the
        // trailing separator, a plain StartsWith(root) would match both, since
        // "data-evil" starts with "data" as a string even though it is a sibling
        // directory, not a descendant.
        return candidateFullPath.Equals(rootFullPath, StringComparison.OrdinalIgnoreCase) ||
               candidateFullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase);
    }
}
