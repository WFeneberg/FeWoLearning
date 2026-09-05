using System.ComponentModel;
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
    /// Explicitly completes WPF's <see cref="ISupportInitialize"/> protocol - what the XAML
    /// parser does automatically around every element it builds. A tree built by plain code
    /// (<c>new ItemsControl()</c>, <c>new Grid()</c>, ...) with ordinary property assignment
    /// never receives this: <c>IsInitialized</c> stays false forever - neither
    /// <see cref="Layout"/> nor <see cref="Pump"/> flips it - and default Style/Template
    /// resolution (a plain <c>new Button()</c> never resolves its default template through
    /// <c>Layout(...)</c> alone) and <c>Grid</c>'s <c>SharedSizeGroup</c> scope registration
    /// are both gated on exactly that flag, with no exception anywhere: the tree just
    /// silently behaves as still mid-construction, so
    /// <c>ItemContainerGenerator.Status</c> stays <c>NotStarted</c> forever and shared column/
    /// row sizes never equalize. Measured directly - <c>HarnessSmokeTests</c>' own Button
    /// dodges this by setting <c>Content</c>, which flips <c>IsInitialized</c> as a side
    /// effect (a bare <c>Tag</c> or <c>Width</c> assignment does not); rows 001-030 never hit
    /// this because they either use a Panel (no Style/Template involved at all) or assign
    /// <c>Style</c> explicitly (bypassing default-style resolution entirely) - rows 031-034
    /// are the first to depend on an unset default Style/Template actually resolving.
    /// One call on the ROOT of an already-built tree is enough: measured directly,
    /// <c>EndInit()</c> reaches every descendant already attached under it at the time it is
    /// called, not just the element it is called on, and it does not matter whether
    /// <c>BeginInit()</c> precedes the tree's construction or is called together with
    /// <c>EndInit()</c> only at the very end. <see cref="Show"/> is the other way to get
    /// this (a real <c>PresentationSource</c> always initializes), but it opens a window -
    /// prefer this for anything that is not otherwise about <c>Loaded</c>, focus or HWND
    /// interop.
    /// </summary>
    protected static void CompleteInitialization(FrameworkElement element)
    {
        var init = (ISupportInitialize)element;
        init.BeginInit();
        init.EndInit();
    }

    /// <summary>
    /// Parks <paramref name="content"/> in an off-screen window so it gets a real
    /// <see cref="PresentationSource"/>. Opt-in: only exercises that need <c>Loaded</c>,
    /// keyboard focus or HWND interop should call it, because it really does create a
    /// window. Closed again by <see cref="Dispose"/>. Returns the <see cref="Window"/>
    /// itself, not <paramref name="content"/>, so a caller that needs the window (rows
    /// 084-088 and any focus row) is not reduced to <c>Window.GetWindow(content)</c>.
    /// </summary>
    protected Window Show(FrameworkElement content)
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

        return window;
    }

    // Virtual so a derived test class can add its own teardown without hiding this one -
    // xunit disposes through the IDisposable interface, so a derived `public new void
    // Dispose()` would compile, look right, and simply never run. An override must call
    // base.Dispose(), or the tracked windows never get closed.
    public virtual void Dispose()
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
