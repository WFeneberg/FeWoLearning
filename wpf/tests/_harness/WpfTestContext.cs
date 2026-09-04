using System.Windows;
using System.Windows.Threading;

namespace FeWoLearning.Wpf.Tests;

/// <summary>
/// Base class for every exercise test. Supplies the three things a windowless WPF tree
/// needs: a real layout pass, a way to drain the dispatcher queue, and an opt-in
/// off-screen window for the few exercises that need a real <see cref="PresentationSource"/>.
/// </summary>
public abstract class WpfTestContext : IDisposable
{
    private readonly List<Window> _windows = [];

    /// <summary>
    /// Runs a real measure/arrange/layout pass. Nothing about a <see cref="FrameworkElement"/>
    /// is trustworthy before this: <c>DesiredSize</c> and <c>ActualWidth</c> are zero and
    /// template children do not exist yet.
    /// </summary>
    protected static void Layout(FrameworkElement element, Size? available = null)
    {
        var size = available ?? new Size(800, 600);

        element.Measure(size);
        element.Arrange(new Rect(size));
        element.UpdateLayout();
    }

    /// <summary>
    /// Drains the dispatcher queue down to <paramref name="priority"/>. Bindings update at
    /// <see cref="DispatcherPriority.DataBind"/>, so a test that asserts before pumping
    /// reads the stale value - the single most common way a WPF test lies. The default
    /// drains everything, including the <see cref="DispatcherPriority.Background"/> work
    /// that <c>CommandManager.InvalidateRequerySuggested</c> posts.
    /// </summary>
    protected static void Pump(DispatcherPriority priority = DispatcherPriority.SystemIdle)
    {
        var frame = new DispatcherFrame();

        Dispatcher.CurrentDispatcher.BeginInvoke(priority, new Action(() => frame.Continue = false));
        Dispatcher.PushFrame(frame);
    }

    /// <summary>
    /// Parks <paramref name="content"/> in an off-screen window so it gets a real
    /// <see cref="PresentationSource"/>. Opt-in: only exercises that need <c>Loaded</c>,
    /// keyboard focus or HWND interop should call it, because it really does create a
    /// window. Closed again by <see cref="Dispose"/>.
    /// </summary>
    protected FrameworkElement Host(FrameworkElement content)
    {
        var window = new Window
        {
            Content = content,
            Width = 800,
            Height = 600,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };

        _windows.Add(window);
        window.Show();
        Pump();

        return content;
    }

    public void Dispose()
    {
        foreach (var window in _windows)
        {
            window.Close();
        }

        _windows.Clear();
        Pump();

        GC.SuppressFinalize(this);
    }
}
