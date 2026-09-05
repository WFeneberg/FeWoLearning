using System.IO;

namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 060 — FilePickerResultStillUntrusted (reference solution).
public static class Ex060_FilePickerResultStillUntrusted
{
    public static bool TryAcceptPickedPath(string pickedPath, string allowedRoot, long maxBytes, out string? rejection)
    {
        ArgumentNullException.ThrowIfNull(pickedPath);
        ArgumentNullException.ThrowIfNull(allowedRoot);

        var fullPicked = Path.GetFullPath(pickedPath);
        var fullRoot = Path.GetFullPath(allowedRoot);
        if (!fullRoot.EndsWith(Path.DirectorySeparatorChar))
            fullRoot += Path.DirectorySeparatorChar;

        if (Directory.Exists(fullPicked))
        {
            rejection = "picked path names a directory, not a file";
            return false;
        }

        if (!File.Exists(fullPicked))
        {
            rejection = "picked path does not exist";
            return false;
        }

        FileSystemInfo? linkTarget;
        try
        {
            linkTarget = new FileInfo(fullPicked).ResolveLinkTarget(returnFinalTarget: true);
        }
        catch (IOException)
        {
            rejection = "picked path's link target could not be resolved";
            return false;
        }

        var resolvedPath = linkTarget?.FullName ?? fullPicked;

        if (!resolvedPath.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase))
        {
            rejection = "picked path resolves outside the allowed root";
            return false;
        }

        var length = new FileInfo(resolvedPath).Length;
        if (length > maxBytes)
        {
            rejection = "picked file exceeds the maximum allowed size";
            return false;
        }

        rejection = null;
        return true;
    }
}
