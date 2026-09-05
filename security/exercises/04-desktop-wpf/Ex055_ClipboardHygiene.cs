namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 055 — ClipboardHygiene (desktop-wpf).
// Goal:   The Windows clipboard is process-global, machine-wide state: anything
//         copied to it can be captured by Clipboard History (Win+V), synced to
//         other devices via Cloud Clipboard, or read by any other process that
//         happens to be watching. CopySecret must place the secret where a normal
//         paste (Ctrl+V) still works, while opting the DataObject out of both of
//         those secondary channels.
// Drills: clipboard as shared state, excluding data from history and cloud sync.
// Passes: attack facts   - the DataObject CopySecret places on the clipboard
//                          carries all three of the registered formats Windows
//                          documents for this, in the values the documentation
//                          gives them: "CanIncludeInClipboardHistory" = false
//                          (keeps it out of Win+V),
//                          "CanUploadToCloudClipboard" = false (keeps it off the
//                          user's other devices) and
//                          "ExcludeClipboardContentFromMonitorProcessing" = true.
//                          Read the names literally - the two "Can..." formats
//                          grant a permission, so denying it is false; the
//                          "Exclude..." one asserts an exclusion, so requesting it
//                          is true.
//         use facts      - Clipboard.GetText() still returns the secret afterwards,
//                          so an ordinary paste is unaffected by the exclusion.
public static class Ex055_ClipboardHygiene
{
    public static void CopySecret(string secret) =>
        throw new NotImplementedException(
            "TODO: Ex055 - place secret on the clipboard as text via a DataObject that also carries " +
            "\"CanIncludeInClipboardHistory\" = false, \"CanUploadToCloudClipboard\" = false and " +
            "\"ExcludeClipboardContentFromMonitorProcessing\" = true");
}
