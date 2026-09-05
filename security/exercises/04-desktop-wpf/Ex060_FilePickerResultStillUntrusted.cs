using System.IO;

namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 060 — FilePickerResultStillUntrusted (desktop-wpf).
// Goal:   A path returned by an OpenFileDialog (or any file picker) came out of a
//         dialog the OS presented, but that does not make it trustworthy input: a
//         symbolic link inside an otherwise-allowed folder can resolve anywhere on
//         disk, a "file" can actually be a directory, and there is no dialog-side
//         limit on size. Every one of those must be re-validated after the dialog
//         returns, exactly as if the path had arrived from an untrusted network
//         request — because to the code accepting it, that is what it is.
// Drills: dialog results are user input, post-dialog validation.
// Passes: attack facts   - a path outside `allowedRoot` is rejected even though a
//                          dialog produced it; a symbolic link whose target resolves
//                          outside `allowedRoot` is rejected even though the link
//                          itself sits inside the root; a file whose length exceeds
//                          `maxBytes` is rejected; a path naming a directory (not a
//                          file) is rejected.
//         use facts      - an ordinary file inside `allowedRoot`, at or under the
//                          size limit, is accepted with `rejection` null; a file
//                          whose size is exactly `maxBytes` is accepted too — the
//                          boundary, not an off-by-one below it.
public static class Ex060_FilePickerResultStillUntrusted
{
    public static bool TryAcceptPickedPath(string pickedPath, string allowedRoot, long maxBytes, out string? rejection) =>
        throw new NotImplementedException(
            "TODO: Ex060 - resolve pickedPath (following any symbolic link) to a full path; reject unless " +
            "the resolved path is an existing file, not a directory, inside allowedRoot, and no larger than maxBytes");
}
