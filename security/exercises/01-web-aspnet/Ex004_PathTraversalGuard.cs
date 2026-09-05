namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 004 — PathTraversalGuard (web-aspnet).
// Goal:   Resolve a caller-supplied relative path under a fixed root directory,
//         rejecting any request whose canonical (fully-normalised) form would
//         land outside that root - including a rooted path, ../ segments, and a
//         path that only escapes once ./ and ../ segments are collapsed.
// Drills: canonicalisation, root containment, safe static file serving.
// Passes: attack facts   - "../secrets.txt", "..\secrets.txt", a rooted path
//                          such as "C:\Windows\win.ini", "subdir/../../outside.txt"
//                          and a path that escapes only after normalisation all
//                          return false with fullPath set to "";
//         use facts      - "report.txt" and "subdir/report.txt" both return true
//                          with a fullPath under the root that Path.GetFullPath
//                          agrees with.
public static class Ex004_PathTraversalGuard
{
    public static bool TryResolve(string rootDirectory, string requestedPath, out string fullPath) =>
        throw new NotImplementedException(
            "TODO: Ex004 - resolve requestedPath under rootDirectory, rejecting any path whose canonical form escapes the root");
}
