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

    /// <summary>
    /// Measure/arrange only. Enough for geometry - NOT enough for guard evaluation (measured:
    /// a CanXxx guard is still unevaluated after Layout alone, see ex023/ex024) or for action
    /// invocation (see <see cref="Show"/>). Guard evaluation needs <see cref="Load"/>.
    /// </summary>
    protected static void Layout(FrameworkElement e)
    {
        e.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        e.Arrange(new Rect(e.DesiredSize));
        e.UpdateLayout();
    }

    /// <summary>
    /// Raises Loaded across the tree. FrameworkElement.LoadedEvent is a *direct* routed
    /// event, so raising it on the root alone never reaches the root's children.
    /// Additionally (measured, ex023/ex024): this is enough for ActionMessage to evaluate a
    /// CanXxx guard and apply IsEnabled - no real window required for that. Actually INVOKING
    /// the action still needs <see cref="Show"/>: guard evaluation and action invocation are
    /// two different thresholds.
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
    /// does not, and neither does raising Loaded by hand -- only a real window does. Guard
    /// evaluation does NOT need this (<see cref="Load"/> alone is enough) - this additionally
    /// unlocks INVOKING the action, which Load alone does not.
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

    // Added for ex046-ex050 (WindowManager/dialogs): WindowManager.ShowDialogAsync is MODAL -
    // it parks the calling thread in a NESTED dispatcher frame (Window.ShowDialog()'s own
    // message loop) until the dialog closes, exactly like ex010/ex041-045's other "this can
    // hang, not just fail" traps, but sharper - here the frame that must be pumped is one
    // ShowDialogAsync itself pushes, so the close cannot be scheduled the normal way (there is
    // no "after Show" moment to hook: by the time ShowDialogAsync returns, it is too late).
    // Measured, repeatedly, on this machine (Caliburn.Micro 5.0.258): the close must be
    // SCHEDULED BEFORE calling ShowDialogAsync, via Dispatcher.BeginInvoke at Background
    // priority, so it runs FROM INSIDE the nested frame once that frame starts pumping -
    // scheduling it after the call (obviously) never runs, because nothing after a blocked
    // call executes until the block ends. Every dialog exercise in the batch goes through
    // this one helper rather than repeating the recipe per test file.
    // protected (not private): ex047's test needs to pass this to the exercise's OWN
    // ShowDialogAsync wrapper explicitly - that exercise's code is what calls ShowDialogAsync,
    // not this harness, so there is no other way to keep that one dialog invisible too.
    protected static readonly IDictionary<string, object> InvisibleDialogSettings = new Dictionary<string, object>
    {
        // Measured to apply and stick (ex049's WindowManagerSettings is the exercise about
        // this dictionary; this default just keeps every OTHER dialog test invisible on a
        // real desktop instead of flashing a centred window for a few hundred milliseconds).
        ["WindowStyle"] = WindowStyle.None,
        ["AllowsTransparency"] = true,
        ["Opacity"] = 0.0,
        ["ShowInTaskbar"] = false,
        ["ShowActivated"] = false,
    };

    /// <summary>
    /// Schedules <c>rootModel.TryCloseAsync(closeWith)</c> to run from INSIDE whatever nested
    /// modal frame is about to start pumping - call this BEFORE starting that frame (i.e.
    /// before calling <c>ShowDialogAsync</c>, directly or through an exercise's own wrapper).
    /// Scheduling it after is measured to never run at all: nothing past a blocked modal call
    /// executes until the block ends. onWindowCaptured, if given, receives the real hosting
    /// <see cref="Window"/> while the dialog is still open - measured,
    /// <c>((IViewAware)vm).GetView()</c> returns null once it has closed, so this is the only
    /// chance to observe it.
    /// </summary>
    protected static void ScheduleTryClose(Screen rootModel, bool? closeWith, Action<Window>? onWindowCaptured = null) =>
        ScheduleFromInsideModalFrame(rootModel, () => rootModel.TryCloseAsync(closeWith), onWindowCaptured);

    /// <summary>
    /// Generalizes <see cref="ScheduleTryClose"/>: schedules an arbitrary closer (any call that
    /// eventually reaches TryCloseAsync - directly, or through an exercise's own method, as
    /// ex048's ConfirmAsync/DeclineAsync/DismissAsync do) to run from inside the nested modal
    /// frame the caller is about to push, capturing the hosting Window first if asked.
    /// </summary>
    protected static void ScheduleFromInsideModalFrame(Screen rootModel, Func<Task> closer, Action<Window>? onWindowCaptured = null)
    {
        // The BeginInvoke callback itself must stay synchronous (Action, not a Task-returning
        // delegate) - a fire-and-forget async lambda here triggers CS4014, and this project
        // holds tests/ to zero warnings. RunAsync carries the actual async work; the two `_ =`
        // discards below are both deliberate - neither DispatcherOperation (itself awaitable,
        // which is why the outer BeginInvoke call also needs one) nor the local function's Task
        // can be awaited from here, since this callback must return so the nested frame the
        // caller is about to push can actually start pumping.
        _ = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new System.Action(() =>
        {
            _ = RunAsync();
        }));

        async Task RunAsync()
        {
            await Task.Yield(); // let the modal frame establish before touching it
            if (onWindowCaptured != null && ((IViewAware)rootModel).GetView() is FrameworkElement v)
                onWindowCaptured(Window.GetWindow(v));
            await closer();
        }
    }

    /// <summary>
    /// Awaits dialogTask, but only up to 8 seconds - a stub that never calls TryCloseAsync
    /// (directly, or via ShowDialogAsync on an un-closed dialog) must fail red with a clear
    /// timeout message instead of hanging the suite; this machine already carries one
    /// unkillable hung test host from an earlier mistake here. Shared by every dialog
    /// exercise's test rather than each bounding its own await.
    /// </summary>
    protected static async Task<T> BoundedDialogAsync<T>(Task<T> dialogTask)
    {
        if (await Task.WhenAny(dialogTask, Task.Delay(TimeSpan.FromSeconds(8))) != dialogTask)
            throw new TimeoutException(
                "A dialog-producing task never completed - a stub that never calls TryCloseAsync leaves the dialog open forever instead of failing.");
        return await dialogTask;
    }

    /// <summary>
    /// The common case built on <see cref="ScheduleTryClose"/> and
    /// <see cref="BoundedDialogAsync{T}"/>: shows rootModel as a modal dialog via a fresh
    /// <see cref="WindowManager"/> and returns both what <c>ShowDialogAsync</c> resolved to and
    /// the real hosting <see cref="Window"/>. settings defaults to
    /// <see cref="InvisibleDialogSettings"/> so a dialog test that is not itself about the
    /// settings dictionary (ex049) never flashes a real, centred window on the developer's
    /// screen. An exercise whose own code is what calls ShowDialogAsync (ex047) uses
    /// ScheduleTryClose/BoundedDialogAsync directly instead - see Ex047's test.
    /// </summary>
    protected static async Task<(bool? Result, Window Window)> ShowDialogAndCloseAsync(
        Screen rootModel, bool? closeWith, IDictionary<string, object>? settings = null)
    {
        Window? capturedWindow = null;
        ScheduleTryClose(rootModel, closeWith, w => capturedWindow = w);

        var result = await BoundedDialogAsync(new WindowManager().ShowDialogAsync(rootModel, null, settings ?? InvisibleDialogSettings));
        return (result, capturedWindow!);
    }

    /// <summary>
    /// Like <see cref="ShowDialogAndCloseAsync"/>, but closes by invoking closer (an exercise's
    /// own method that is itself expected to reach TryCloseAsync) rather than calling
    /// TryCloseAsync directly - for ex048, whose whole point is that DIFFERENT calls to
    /// TryCloseAsync(bool?) are the exercise, not the show/await plumbing around them.
    /// </summary>
    protected static async Task<(bool? Result, Window Window)> ShowDialogInvokingAsync(
        Screen rootModel, Func<Task> closer, IDictionary<string, object>? settings = null)
    {
        Window? capturedWindow = null;
        ScheduleFromInsideModalFrame(rootModel, closer, w => capturedWindow = w);

        var result = await BoundedDialogAsync(new WindowManager().ShowDialogAsync(rootModel, null, settings ?? InvisibleDialogSettings));
        return (result, capturedWindow!);
    }
}
