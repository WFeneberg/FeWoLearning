using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Tests;

/// <summary>
/// Base class for exercises with a view. Valid ONLY under [WpfFact]/[WpfTheory]:
/// XamlPlatformProvider captures Dispatcher.CurrentDispatcher in its constructor, so it
/// has to be built on the STA test thread that will actually pump it.
/// </summary>
public abstract class CaliburnViewContext : CaliburnCoreContext, IDisposable
{
    readonly List<Window> _windows = [];

    protected CaliburnViewContext() => PlatformProvider.Current = new XamlPlatformProvider();

    /// <summary>Measure/arrange only. Enough for geometry and for guard evaluation.</summary>
    protected static void Layout(FrameworkElement e)
    {
        e.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        e.Arrange(new Rect(e.DesiredSize));
        e.UpdateLayout();
    }

    /// <summary>
    /// Raises Loaded across the tree. FrameworkElement.LoadedEvent is a *direct* routed
    /// event, so raising it on the root alone never reaches the root's children.
    /// Use this only when a Loaded callback is the subject; actions need <see cref="Show"/>.
    /// </summary>
    protected static void Load(FrameworkElement root)
    {
        Layout(root);
        foreach (var e in SelfAndDescendants(root))
            e.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent, e));
    }

    static IEnumerable<FrameworkElement> SelfAndDescendants(DependencyObject root)
    {
        if (root is FrameworkElement fe) yield return fe;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            foreach (var child in SelfAndDescendants(VisualTreeHelper.GetChild(root, i)))
                yield return child;
    }

    /// <summary>
    /// Hosts the view in a real window, parked off-screen at zero opacity, closed on
    /// dispose. THIS IS THE ONLY WAY TO EXERCISE AN ACTION: Caliburn's actions ride on
    /// Microsoft.Xaml.Behaviors triggers, which refuse to resolve their source until the
    /// element has a PresentationSource. Measure/Arrange does not supply one, ApplyTemplate
    /// does not, and neither does raising Loaded by hand -- only a real window does.
    /// </summary>
    protected Window Show(FrameworkElement view)
    {
        var w = new Window
        {
            Content = view,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Opacity = 0,
            Width = 400,
            Height = 300,
            Left = -32000,
            Top = -32000,
        };
        _windows.Add(w);
        w.Show();
        Pump(DispatcherPriority.Loaded);
        return w;
    }

    /// <summary>Drains the dispatcher queue. Assert only after pumping.</summary>
    protected static void Pump(DispatcherPriority priority = DispatcherPriority.Background) =>
        Dispatcher.CurrentDispatcher.Invoke(() => { }, priority);

    // Virtual so a derived test class can add its own teardown without hiding this one -
    // an override MUST call base.Dispose(), or the tracked windows never get closed.
    public virtual void Dispose()
    {
        foreach (var w in _windows) w.Close();
        GC.SuppressFinalize(this);
    }
}
