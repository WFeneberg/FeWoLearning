using System.Windows.Threading;

namespace FeWoLearning.Security.Tests.Harness;

public static class WpfPump
{
    // Drains the dispatcher queue down to `priority`. Bindings update at
    // DispatcherPriority.DataBind, so a test that mutates a source and asserts
    // immediately reads the stale value - call this in between.
    public static void Pump(DispatcherPriority priority = DispatcherPriority.Loaded)
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(
            priority,
            new DispatcherOperationCallback(f => { ((DispatcherFrame)f).Continue = false; return null; }),
            frame);
        Dispatcher.PushFrame(frame);
    }
}
