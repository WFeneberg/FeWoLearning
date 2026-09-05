using System.IO;
using System.IO.Compression;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 047 — ZipSlipExtraction (desktop-core).
// Goal:   Extract a zip archive to a directory without letting any entry write
//         outside that directory - the classic "zip slip" vulnerability, where
//         an entry name like "../../evil.txt" or an absolute path escapes the
//         intended destination the moment its path is naively combined with it.
// Drills: archive entry path containment, absolute and relative escapes.
// Passes: attack facts   - an entry named "../escaped.txt" is not written, and
//                          no file appears anywhere outside destinationDirectory
//                          (verified by listing the destination's parent, not just
//                          checking one expected path); an entry with an absolute
//                          path is not written; an entry named
//                          "sub/../../escaped.txt" (which still resolves outside
//                          the destination once "sub" cancels against the first
//                          "..") is not written either;
//         use facts      - ordinary entries "a.txt" and "sub/b.txt" are written
//                          with their correct content, and the returned list
//                          names exactly the files that were written - nothing
//                          more, nothing less.
public static class Ex047_ZipSlipExtraction
{
    public static IReadOnlyList<string> ExtractTo(Stream archive, string destinationDirectory) =>
        throw new NotImplementedException(
            "TODO: Ex047 - for each zip entry, resolve its full destination path and reject (skip) any entry " +
            "whose resolved path does not fall under destinationDirectory; write the rest and return their paths");
}
