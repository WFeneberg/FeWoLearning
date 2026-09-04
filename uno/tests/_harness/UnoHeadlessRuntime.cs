using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Tests;

/// <summary>
/// Boots Uno's Skia runtime inside the test host so exercises can create real
/// <see cref="UIElement"/> trees, lay them out and evaluate bindings without a window.
/// Test-infrastructure only - no exercise depends on this file.
/// </summary>
/// <remarks>
/// Uno's Skia dispatcher is normally initialised by a platform head (Win32, X11, WPF...)
/// which owns a message loop. There is no headless head, so this type installs the two
/// hooks a head would install:
/// <list type="bullet">
///   <item><c>NativeDispatcher.HasThreadAccessOverride</c> - always true: the test thread
///     *is* the UI thread here.</item>
///   <item><c>NativeDispatcher.DispatchOverride</c> - runs queued work inline and
///     synchronously, so a test never has to pump a loop or await a frame.</item>
/// </list>
/// Both fields are <c>internal</c> in Uno.UI.Dispatching, hence the reflection. That is
/// the one fragile spot in this track: <c>global.json</c> pins <c>Uno.Sdk</c>, and a
/// version bump can rename or remove these fields. If every test suddenly fails with
/// <c>NullReferenceException</c> out of <c>NativeDispatcher</c>, re-check the field names
/// against the pinned Uno release - see uno/README.md.
/// </remarks>
internal static class UnoHeadlessRuntime
{
    private static readonly Lock Gate = new();
    private static readonly Queue<Action> _deferred = new();
    private static bool _booted;
    private static bool _pumping;

    [ModuleInitializer]
    internal static void Boot()
    {
        lock (Gate)
        {
            if (_booted)
            {
                return;
            }

            _booted = true;

            InstallDispatcherHooks();
            InstallIcuData();
            StartApplication();
        }
    }

    private static void InstallDispatcherHooks()
    {
        var dispatching = typeof(Microsoft.UI.Dispatching.DispatcherQueue).Assembly;
        var dispatcher = dispatching.GetType("Uno.UI.Dispatching.NativeDispatcher", throwOnError: true)!;
        var priority = dispatching.GetType("Uno.UI.Dispatching.NativeDispatcherPriority", throwOnError: true)!;

        Field(dispatcher, "HasThreadAccessOverride").SetValue(null, (Func<bool>)(() => true));

        var inline = typeof(UnoHeadlessRuntime)
            .GetMethod(nameof(RunInline), BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(priority);
        var dispatchField = Field(dispatcher, "DispatchOverride");
        dispatchField.SetValue(null, Delegate.CreateDelegate(dispatchField.FieldType, inline));

        static FieldInfo Field(Type owner, string name) =>
            owner.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException(
                $"Uno's {owner.Name}.{name} hook is gone - the pinned Uno.Sdk version in uno/global.json "
                + "no longer matches this harness. See the remarks in UnoHeadlessRuntime.");
    }

    /// <summary>
    /// Stands in for a head's message loop: queued work runs immediately, so a test never
    /// has to pump or await a frame.
    /// </summary>
    /// <remarks>
    /// The re-entrancy guard is not optional. Uno hands this method its own pump, and work
    /// running inside the pump can enqueue more - an awaited continuation resuming inside a
    /// cancellation callback is enough. Calling the pump again from inside itself recurses
    /// until the stack runs out, and a StackOverflowException takes the whole test host
    /// down with no failing test to point at. So a nested call defers, and the outer call
    /// drains what piled up.
    /// </remarks>
    private static void RunInline<TPriority>(Action work, TPriority priority)
    {
        if (_pumping)
        {
            _deferred.Enqueue(work);
            return;
        }

        _pumping = true;

        try
        {
            work();

            while (_deferred.Count > 0)
            {
                _deferred.Dequeue()();
            }
        }
        finally
        {
            _pumping = false;
        }
    }

    /// <summary>
    /// Uno shapes and measures text through ICU. Its loader reads <c>icudt.dat</c> out of
    /// an embedded resource, and only an Uno *head* assembly carries one - which is why
    /// the test project sets <c>IsUnoHead</c> and why this assembly is the one handed over.
    /// </summary>
    private static void InstallIcuData()
    {
        // Note the "+ICU" nested-type syntax: asking UnicodeText for its nested types the
        // usual way makes the runtime resolve *all* of them, and one of those siblings
        // drags in an assembly that is not present in a test host.
        var icu = typeof(UIElement).Assembly.GetType("Microsoft.UI.Xaml.Documents.UnicodeText+ICU");

        icu?.GetMethod("SetDataAssembly", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?.Invoke(null, [typeof(UnoHeadlessRuntime).Assembly]);
    }

    /// <summary>
    /// Templated controls resolve their default style off <c>Application.Current</c>, so
    /// without an application a <c>Button</c> has a null Template and measures to 0x0.
    /// <c>Application.Start</c> returns immediately here because the inline dispatcher
    /// installed above never blocks on a message loop.
    /// </summary>
    private static void StartApplication()
    {
        Application.Start(_ => new HarnessApplication());

        var fluent = Type.GetType("Microsoft.UI.Xaml.Controls.XamlControlsResources, Uno.UI.FluentTheme");
        if (fluent is not null && Activator.CreateInstance(fluent) is ResourceDictionary theme)
        {
            Application.Current.Resources.MergedDictionaries.Add(theme);
        }
    }

    private sealed partial class HarnessApplication : Application;
}
