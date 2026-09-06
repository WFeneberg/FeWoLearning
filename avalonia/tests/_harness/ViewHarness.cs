using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;

namespace FeWoLearning.Avalonia.Tests;

public static class ViewHarness
{
    /// <summary>
    /// Puts <paramref name="view"/> in a headless Window and shows it, which applies
    /// control templates and drives a full measure/arrange pass.
    ///
    /// Do NOT replace this with a bare Measure/Arrange call. A UserControl's XAML
    /// lives in its Content, hosted by a ContentPresenter from its control template;
    /// without an applied template the control reports its own arranged size while
    /// every child stays 0,0,0,0. ApplyTemplate() before Measure/Arrange does not
    /// fix it either. This was measured, not assumed.
    ///
    /// A headless Window's client area equals its requested Width/Height exactly,
    /// so geometry assertions against these sizes are deterministic.
    /// </summary>
    public static TView Show<TView>(TView view, double width = 400, double height = 300)
        where TView : Control
    {
        var window = new Window { Content = view, Width = width, Height = height };
        window.Show();
        return view;
    }

    /// <summary>
    /// Like <see cref="Show{TView}"/>, but hands back the <see cref="Window"/> rather
    /// than the view, because that is what the headless input extensions take:
    /// <c>window.MouseDown(...)</c>, <c>window.KeyPressQwerty(...)</c> and friends all
    /// extend <c>TopLevel</c>.
    ///
    /// Show exactly ONE window per test when you care about rendering. Measured: only
    /// the first window shown in a test actually paints on a plain
    /// <c>RunJobs()</c>; a second window in the same test rendered nothing at all
    /// until the render timer was ticked. Input is unaffected.
    /// </summary>
    public static Window ShowWindow(Control view, double width = 400, double height = 300)
    {
        var window = new Window { Content = view, Width = width, Height = height };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    /// <summary>
    /// Drives one render pass and drains the dispatcher.
    ///
    /// Both halves are needed and the order matters. There is no render loop in a
    /// headless test, so <c>ForceRenderTimerTick</c> is what makes the compositor
    /// consider a frame at all; <c>RunJobs</c> then lets the work it queued run.
    /// Measured with a control counting its own <c>Render</c> calls: pumping while
    /// nothing is dirty renders NOTHING (the count stayed at 1 across three pumps),
    /// each <c>InvalidateVisual</c> yields exactly one further render, and five
    /// invalidations before a single pump coalesce into one.
    ///
    /// Note that this does not make what was drawn observable - the headless backend
    /// still discards draw commands. See the README section on rendering.
    /// </summary>
    public static void PumpRender()
    {
        AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
        Dispatcher.UIThread.RunJobs();
    }

    /// <summary>
    /// True when the tests were built with -p:UseSolutions=true, detected from the
    /// content assembly that actually got loaded rather than from a compile symbol.
    ///
    /// Anchored on TrackMarker rather than any single exercise type, so this stays
    /// correct independent of which exercises exist yet.
    /// </summary>
    public static bool SolutionsMode =>
        typeof(FeWoLearning.Avalonia.Exercises.TrackMarker).Assembly.GetName().Name
            == "FeWoLearning.Avalonia.Solutions";
}
