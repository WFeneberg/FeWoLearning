using System.IO;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 004 — PathTraversalGuard (reference solution).
public static class Ex004_PathTraversalGuard
{
    public static bool TryResolve(string rootDirectory, string requestedPath, out string fullPath)
    {
        fullPath = "";

        // A rooted request ("C:\Windows\win.ini", "\\server\share\...") names its own
        // location outright and never needed the root at all - reject before it ever
        // reaches Path.Combine, where Path.Combine would just return it unchanged.
        if (string.IsNullOrEmpty(requestedPath) || Path.IsPathRooted(requestedPath))
            return false;

        var root = Path.GetFullPath(rootDirectory);
        var rootWithSeparator = root.EndsWith(Path.DirectorySeparatorChar)
            ? root
            : root + Path.DirectorySeparatorChar;

        // The containment check happens AFTER GetFullPath, not on the raw string: a
        // textual scan for ".." would miss "a/b/../../../secrets.txt", which contains
        // no ".." at its start and only reveals the escape once "." and ".." segments
        // are actually collapsed.
        var candidate = Path.GetFullPath(Path.Combine(root, requestedPath));

        if (!candidate.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase))
            return false;

        fullPath = candidate;
        return true;
    }
}
