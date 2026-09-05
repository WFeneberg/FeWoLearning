using System.Runtime.InteropServices;
using System.Windows;
using FeWoLearning.Security.Exercises.DesktopWpf;

namespace FeWoLearning.Security.Tests.DesktopWpf;

// Touches the REAL system clipboard, which is shared with the developer's own
// session - see security/README.md. The constructor/Dispose pair captures and
// restores whatever the clipboard held before this test ran, and every clipboard
// read or write goes through WithRetry, because the Win32 clipboard can be
// momentarily locked by another process (clipboard-history capture, an antivirus
// scanner, another app's own Ctrl+C) and an unguarded call throws COMException
// intermittently. The assembly-level [assembly: Parallelization(Mode = None)] in
// tests/AssemblyInfo.cs is what keeps this test from racing itself or Ex050's
// named-pipe test across threads.
public class Ex055_ClipboardHygieneTests : IDisposable
{
    private const string Secret = "s3cr3t-marker-7f1c";

    private readonly IDataObject? _priorClipboard;

    public Ex055_ClipboardHygieneTests()
    {
        _priorClipboard = WithRetry(Clipboard.GetDataObject);
    }

    public void Dispose()
    {
        WithRetry<object?>(() =>
        {
            if (_priorClipboard is not null)
            {
                Clipboard.SetDataObject(_priorClipboard, copy: true);
            }
            else
            {
                Clipboard.Clear();
            }

            return null;
        });
    }

    [WpfFact]
    public void Attack_The_DataObject_Excludes_The_Secret_From_History_And_Cloud_Sync()
    {
        Ex055_ClipboardHygiene.CopySecret(Secret);

        var data = WithRetry(Clipboard.GetDataObject)!;

        Assert.Equal(false, data.GetData("CanIncludeInClipboardHistory"));
        Assert.Equal(false, data.GetData("ExcludeClipboardContentFromMonitorProcessing"));
    }

    [WpfFact]
    public void Use_GetText_Still_Returns_The_Secret_So_Paste_Still_Works()
    {
        Ex055_ClipboardHygiene.CopySecret(Secret);

        var pasted = WithRetry(Clipboard.GetText);

        Assert.Equal(Secret, pasted);
    }

    private static T WithRetry<T>(Func<T> action)
    {
        const int maxAttempts = 10;
        COMException? last = null;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                return action();
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
