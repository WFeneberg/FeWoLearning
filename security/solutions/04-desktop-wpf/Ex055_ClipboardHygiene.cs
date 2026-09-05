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

        // Both formats are booleans: Windows' clipboard history (Win+V) and Cloud
        // Clipboard sync each check for them and skip this content when they are
        // present and false. Neither format has any effect on Clipboard.GetText(),
        // so a normal paste is unaffected.
        data.SetData("CanIncludeInClipboardHistory", false);
        data.SetData("ExcludeClipboardContentFromMonitorProcessing", false);

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
