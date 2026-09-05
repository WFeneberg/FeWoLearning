using System.Runtime.InteropServices;
using System.Windows;

namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 055 — ClipboardHygiene (reference solution).
public static class Ex055_ClipboardHygiene
{
    public static void CopySecret(string secret)
    {
        ArgumentNullException.ThrowIfNull(secret);

        var data = new DataObject();
        data.SetData(DataFormats.UnicodeText, secret);

        // The three registered clipboard formats Windows documents for this, in
        // the values the documentation gives them. Read the names literally: the
        // two "Can..." formats grant a permission, so denying it means false;
        // "ExcludeClipboardContentFromMonitorProcessing" asserts an exclusion, so
        // requesting it means true. CanIncludeInClipboardHistory keeps the value
        // out of Win+V, CanUploadToCloudClipboard keeps it off the user's other
        // devices, and the exclusion format asks clipboard monitors in general not
        // to process it. None of them affects Clipboard.GetText(), so an ordinary
        // Ctrl+V paste still works.
        data.SetData("CanIncludeInClipboardHistory", false);
        data.SetData("CanUploadToCloudClipboard", false);
        data.SetData("ExcludeClipboardContentFromMonitorProcessing", true);

        // The Win32 clipboard is a single, machine-wide resource that any other
        // process (including the OS's own clipboard-history service) can hold open
        // for a moment; SetDataObject can throw COMException(CLIPBRD_E_CANT_OPEN)
        // when that happens. A handful of short, bounded retries rides that out
        // without ever hanging.
        WithClipboardRetry(() => Clipboard.SetDataObject(data, copy: true));
    }

    internal static void WithClipboardRetry(Action action)
    {
        const int maxAttempts = 10;
        COMException? last = null;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                action();
                return;
            }
            catch (COMException ex)
            {
                last = ex;
                Thread.Sleep(25);
            }
        }

        throw new InvalidOperationException("Could not access the clipboard after repeated attempts.", last);
    }
}
