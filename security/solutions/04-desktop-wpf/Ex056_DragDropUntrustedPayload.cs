using System.IO;
using System.Windows;

namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 056 — DragDropUntrustedPayload (reference solution).
public static class Ex056_DragDropUntrustedPayload
{
    public static IReadOnlyList<string> AcceptableFiles(
        IDataObject data, string allowedRoot, IReadOnlyCollection<string> allowedExtensions)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(allowedRoot);
        ArgumentNullException.ThrowIfNull(allowedExtensions);

        if (!data.GetDataPresent(DataFormats.FileDrop))
        {
            return Array.Empty<string>();
        }

        if (data.GetData(DataFormats.FileDrop) is not string[] paths)
        {
            return Array.Empty<string>();
        }

        var root = Path.GetFullPath(allowedRoot);
        if (!root.EndsWith(Path.DirectorySeparatorChar))
        {
            root += Path.DirectorySeparatorChar;
        }

        var accepted = new List<string>();
        foreach (var path in paths)
        {
            // Each path is judged independently: one bad entry in the drop must
            // never disqualify the good ones sitting alongside it.
            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(path);
            }
            catch (Exception ex) when (ex is ArgumentException or PathTooLongException or NotSupportedException)
            {
                continue;
            }

            if (!fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var extension = Path.GetExtension(fullPath);
            var extensionAllowed = allowedExtensions.Any(
                allowed => string.Equals(allowed, extension, StringComparison.OrdinalIgnoreCase));
            if (!extensionAllowed)
            {
                continue;
            }

            accepted.Add(fullPath);
        }

        return accepted;
    }
}
