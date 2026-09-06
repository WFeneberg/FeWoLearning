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
    // it parks the calling thread inside Window.ShowDialog()'s own nested Dispatcher.PushFrame
    // loop until the dialog closes, exactly like ex010/ex041-045's other "this can hang, not
    // just fail" traps, but sharper - here the frame that must be pumped is one ShowDialogAsync
    // itself pushes, so the close cannot be scheduled the normal way (there is no "after Show"
    // moment to hook: by the time ShowDialogAsync returns, it is too late).
    // Measured, repeatedly, on this machine (Caliburn.Micro 5.0.258): CreateWindowAsync
    // completes SYNCHRONOUSLY, so the calling thread is genuinely blocked inside ShowDialog()'s
    // managed message pump by the time control would otherwise return - the close must
    // therefore be SCHEDULED BEFORE calling ShowDialogAsync, via Dispatcher.BeginInvoke at
    // Background priority, so it runs FROM INSIDE that pump once it starts (and it is precisely
    // BECAUSE that pump keeps pumping that a later-queued timeout continuation can run at all -
    // see BoundedDialogAsync below). Scheduling the close after the call is measured to never
    // run at all: nothing past a blocked modal call executes until the block ends. Every dialog
    // exercise in the batch goes through this one set of helpers rather than repeating the
    // recipe per test file.
    //
    // LOAD-BEARING INVARIANT: nothing in this file's await chains may use ConfigureAwait(false).
    // Window.Close() (and everything else here that touches WPF objects) must run back on the
    // STA thread that owns them - off that thread it throws, rather than closing anything.
    //
    // protected (not private): ex047's test needs to pass this to the exercise's OWN
    // ShowDialogAsync wrapper explicitly - that exercise's code is what calls ShowDialogAsync,
    // not this harness, so there is no other way to keep that one dialog invisible too. It is a
    // METHOD, not a shared field: WindowManager applies these by mutating the dictionary handed
    // to it (measured), so a shared mutable static here would let one test's settings leak into
    // every later dialog - each call gets its own fresh copy instead.
    protected static IDictionary<string, object> InvisibleDialogSettings() => new Dictionary<string, object>
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

    /// <summary>Whatever Window currently hosts rootModel's view, or null if none does right
    /// now (no view attached, or the view is not currently inside any Window). Re-derived fresh
    /// on every call rather than cached, because a cached value can go stale in exactly the
    /// scenario that matters most here - see BoundedDialogAsync's doc comment.</summary>
    static Window? CurrentHostingWindowOf(Screen rootModel) =>
        ((IViewAware)rootModel).GetView() is FrameworkElement v ? Window.GetWindow(v) : null;

    /// <summary>
    /// Schedules <c>rootModel.TryCloseAsync(closeWith)</c> to run from INSIDE whatever nested
    /// modal frame is about to start pumping - call this BEFORE starting that frame (i.e.
    /// before calling <c>ShowDialogAsync</c>, directly or through an exercise's own wrapper).
    /// Scheduling it after is measured to never run at all: nothing past a blocked modal call
    /// executes until the block ends. onWindowCaptured, if given, receives the real hosting
    /// <see cref="Window"/> while the dialog is still open (or null if none was found yet) -
    /// measured, <c>((IViewAware)vm).GetView()</c> returns null once the dialog has closed, so
    /// this is the only chance to observe it this way.
    /// </summary>
    protected static void ScheduleTryClose(Screen rootModel, bool? closeWith, Action<Window?>? onWindowCaptured = null) =>
        ScheduleFromInsideModalFrame(rootModel, () => rootModel.TryCloseAsync(closeWith), onWindowCaptured);

    /// <summary>
    /// Generalizes <see cref="ScheduleTryClose"/>: schedules an arbitrary closer (any call that
    /// eventually reaches TryCloseAsync - directly, or through an exercise's own method, as
    /// ex048's ConfirmAsync/DeclineAsync/DismissAsync do) to run from inside the nested modal
    /// frame the caller is about to push, capturing the hosting Window first if asked.
    ///
    /// CRITICAL (measured the hard way - this is what an unfinished stub actually does): if
    /// anything in here throws (e.g. an exercise's own method is still a bare
    /// NotImplementedException) BEFORE ever reaching TryCloseAsync, the window never closes,
    /// and Window.ShowDialog()'s nested pump then waits FOREVER - a Task-level
    /// Task.WhenAny/Task.Delay race in whatever is awaiting the dialog does NOT reach into that
    /// pump and stop it; the pump only ever ends when the WINDOW actually closes. So the WHOLE
    /// body below (the capture too, not just the closer call) is force-close-on-failure, and
    /// reports the failure via onCloserFailed rather than letting it vanish inside this
    /// fire-and-forget callback - a caller that ignores onCloserFailed would otherwise see
    /// whatever ShowDialogAsync resolves a forced, DialogResult-less Close() to (false) and
    /// could wrongly read that as a legitimate result instead of the stub failure it actually was.
    /// </summary>
    protected static void ScheduleFromInsideModalFrame(
        Screen rootModel, Func<Task> closer, Action<Window?>? onWindowCaptured = null, Action<Exception>? onCloserFailed = null)
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
            try
            {
                onWindowCaptured?.Invoke(CurrentHostingWindowOf(rootModel));
                await closer();
            }
            catch (Exception ex)
            {
                onCloserFailed?.Invoke(ex);
                // Re-derive rather than trust a variable this try block may never have reached -
                // this must force the pump closed even if the capture line above is what threw.
                CurrentHostingWindowOf(rootModel)?.Close();
            }
        }
    }

    /// <summary>
    /// Awaits dialogTask, but only up to 8 seconds - a stub that reaches TryCloseAsync's caller
    /// correctly but simply never reaches TryCloseAsync itself (e.g. an empty method body that
    /// returns Task.CompletedTask without doing anything, or one that awaits something before
    /// ShowDialogAsync is even called) must fail red with a clear timeout message instead of
    /// hanging the suite; this machine already carries one unkillable hung test host from an
    /// earlier mistake here. On timeout this ALSO force-closes whatever Window currently hosts
    /// rootModel - re-derived fresh AT THE MOMENT OF TIMEOUT rather than relying on any earlier
    /// capture, because a correct-but-differently-shaped implementation (one with an `await`
    /// before ever calling ShowDialogAsync) can make an earlier capture attempt run before the
    /// dialog exists at all, leaving it permanently null; measured, while a dialog is genuinely
    /// open <c>GetView()</c> yields the hosting Window in both the wrapped-UserControl and the
    /// Window-derived case, so re-deriving here always finds it if the dialog is still open.
    /// Giving up on WAITING does not, by itself, make Window.ShowDialog()'s nested pump return -
    /// only the window actually closing does that.
    /// </summary>
    protected static async Task<T> BoundedDialogAsync<T>(Task<T> dialogTask, Screen rootModel, string because = "a dialog to close")
    {
        var winner = await Task.WhenAny(dialogTask, Task.Delay(TimeSpan.FromSeconds(8)));
        if (winner != dialogTask)
        {
            CurrentHostingWindowOf(rootModel)?.Close();
            await Task.WhenAny(dialogTask, Task.Delay(TimeSpan.FromSeconds(3)));
            Assert.True(dialogTask.IsCompleted,
                $"Timed out waiting for {because} - a stub that never reaches TryCloseAsync leaves the dialog open forever instead of failing, even after a forced Close().");
        }
        return await dialogTask;
    }

    /// <summary>
    /// Like <see cref="ShowDialogAndCloseAsync"/>, but closes by invoking closer (an exercise's
    /// own method that is itself expected to reach TryCloseAsync) rather than calling
    /// TryCloseAsync directly - for ex048, whose whole point is that DIFFERENT calls to
    /// TryCloseAsync(bool?) are the exercise, not the show/await plumbing around them. If closer
    /// throws (an unfinished stub), that failure is rethrown here rather than allowed to vanish -
    /// see <see cref="ScheduleFromInsideModalFrame"/>'s doc comment for why.
    /// </summary>
    protected static async Task<(bool? Result, Window Window)> ShowDialogInvokingAsync(
        Screen rootModel, Func<Task> closer, IDictionary<string, object>? settings = null)
    {
        Window? capturedWindow = null;
        Exception? closerFailure = null;
        ScheduleFromInsideModalFrame(rootModel, closer, w => capturedWindow = w, ex => closerFailure = ex);

        var result = await BoundedDialogAsync(
            new WindowManager().ShowDialogAsync(rootModel, null, settings ?? InvisibleDialogSettings()),
            rootModel,
            "ShowDialogAsync (invoking the exercise's own closer)");

        if (closerFailure != null)
            throw new InvalidOperationException("The exercise's own close method threw before the dialog could close.", closerFailure);

        return (result, capturedWindow ?? throw new InvalidOperationException(
            "The dialog closed without ever capturing its hosting Window - GetView() never returned one while it was open."));
    }

    /// <summary>
    /// The common case: shows rootModel as a modal dialog via a fresh <see cref="WindowManager"/>
    /// and returns both what <c>ShowDialogAsync</c> resolved to and the real hosting
    /// <see cref="Window"/>. Built directly on <see cref="ShowDialogInvokingAsync"/> (closer is
    /// just <c>rootModel.TryCloseAsync(closeWith)</c>) so both share the exact same
    /// failure-reporting path - a throwing TryCloseAsync is symmetric with a throwing exercise
    /// method, not silently force-closed into a misleadingly clean `false`. settings defaults to
    /// <see cref="InvisibleDialogSettings"/> so a dialog test that is not itself about the
    /// settings dictionary (ex049) never flashes a real, centred window on the developer's
    /// screen. An exercise whose own code is what calls ShowDialogAsync (ex047) uses
    /// ScheduleTryClose/BoundedDialogAsync directly instead - see Ex047's test.
    /// </summary>
    protected static Task<(bool? Result, Window Window)> ShowDialogAndCloseAsync(
        Screen rootModel, bool? closeWith, IDictionary<string, object>? settings = null) =>
        ShowDialogInvokingAsync(rootModel, () => rootModel.TryCloseAsync(closeWith), settings);
}
