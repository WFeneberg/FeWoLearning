using System.IO;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 048 — PathCanonicalization (desktop-core).
// Goal:   Decide whether a candidate path truly resolves to a location under a
//         root directory, after full canonicalization - not by comparing raw
//         strings, which is where every one of this exercise's attack facts
//         lives.
// Drills: full-path containment, UNC and device-name traps, alternate streams.
// Passes: attack facts, all false - a sibling directory whose name merely
//                          *starts with* the root's name (candidate under
//                          "C:\data-evil" against root "C:\data" - the classic
//                          `StartsWith` bug); a path that escapes via "..";
//                          a UNC path (\\server\share\...); a device path
//                          (\\?\C:\...) that escapes the root; a path with a
//                          trailing NTFS alternate-data-stream suffix;
//         use facts      - the root itself is inside; a nested file is inside;
//                          and a nested path written with forward slashes
//                          instead of backslashes is still recognised as
//                          inside.
public static class Ex048_PathCanonicalization
{
    public static bool IsInside(string root, string candidate) =>
        throw new NotImplementedException(
            "TODO: Ex048 - fully canonicalize both root and candidate (rejecting UNC/device paths and alternate " +
            "data streams outright) and check the canonical candidate falls under the canonical root, separator-safe");
}
