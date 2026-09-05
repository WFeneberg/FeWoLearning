using System.IO;
using System.Windows;

namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 056 — DragDropUntrustedPayload (desktop-wpf).
// Goal:   A file dropped onto a WPF window is user input in exactly the same sense
//         as a text box's contents - it can name any path on the filesystem and any
//         extension, chosen by whoever is dragging, not by the application. Before
//         acting on a drop (opening it, copying it, running it), every path in it
//         must be checked against an allowed root directory and an extension
//         allowlist.
// Drills: validating dropped formats and paths before acting.
// Passes: attack facts   - a drop naming a `.exe` file yields an empty list; a drop
//                          naming a path outside `allowedRoot` yields an empty list;
//                          a DataObject carrying no FileDrop format at all (e.g. a
//                          plain-text drop) yields an empty list rather than
//                          throwing.
//         use facts      - a drop of two allowed files under the root yields both,
//                          in the order they were dropped; a MIXED drop (one
//                          allowed file, one disallowed) yields only the allowed
//                          file rather than rejecting the whole batch - the fact
//                          that rules out an all-or-nothing implementation.
public static class Ex056_DragDropUntrustedPayload
{
    public static IReadOnlyList<string> AcceptableFiles(
        IDataObject data, string allowedRoot, IReadOnlyCollection<string> allowedExtensions) =>
        throw new NotImplementedException(
            "TODO: Ex056 - read DataFormats.FileDrop from data (returning empty, not throwing, when it is " +
            "absent), then keep only the paths that resolve under allowedRoot and whose extension is in " +
            "allowedExtensions, preserving order and evaluating each path independently");
}
