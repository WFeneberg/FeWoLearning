using Avalonia.Controls;

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
