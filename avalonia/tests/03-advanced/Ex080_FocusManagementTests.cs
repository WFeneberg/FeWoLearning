using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex080_FocusManagementTests
{
    private static (Ex080_FocusManagement Panel, Window Window) Shown()
    {
        var panel = new Ex080_FocusManagement();
        var window = ViewHarness.ShowWindow(panel, 240, 220);
        return (panel, window);
    }

    private static string Focused(Window window) =>
        window.FocusManager?.GetFocusedElement() is ContentControl control
            ? $"{control.Content}"
            : "none";

    private static void Tab(Window window, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        window.KeyPressQwerty(PhysicalKey.Tab, modifiers);
        Dispatcher.UIThread.RunJobs();
    }

    // The traversal order, and the point of the exercise: it is Beta, Alpha, Delta
    // even though the children are added Alpha, Beta, Gamma, Delta - so it cannot
    // have come from the tree order - and it wraps back to Beta rather than
    // stopping. Gamma never appears.
    [AvaloniaFact]
    public void Tab_Visits_The_Configured_Order_And_Wraps()
    {
        var (panel, window) = Shown();

        panel.Beta.Focus();
        Dispatcher.UIThread.RunJobs();

        var visited = new List<string> { Focused(window) };

        for (var i = 0; i < 3; i++)
        {
            Tab(window);
            visited.Add(Focused(window));
        }

        Assert.Equal(["Beta", "Alpha", "Delta", "Beta"], visited);
    }

    [AvaloniaFact]
    public void Shift_Tab_Walks_The_Same_Order_Backwards()
    {
        var (panel, window) = Shown();

        panel.Delta.Focus();
        Dispatcher.UIThread.RunJobs();

        Tab(window, RawInputModifiers.Shift);
        Assert.Equal("Alpha", Focused(window));

        Tab(window, RawInputModifiers.Shift);
        Assert.Equal("Beta", Focused(window));
    }

    // The distinction the exercise exists for. Gamma is not a tab stop, so
    // traversal skips it - yet Focus() on it succeeds and it really does hold the
    // focus. An answer that reaches for Focusable instead fails here, because
    // Focusable false would make this return false.
    [AvaloniaFact]
    public void Gamma_Is_Skipped_By_Tab_Yet_Still_Focusable_By_Code()
    {
        var (panel, window) = Shown();

        Assert.False(panel.Gamma.IsTabStop);
        Assert.True(panel.Gamma.Focus());
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Gamma", Focused(window));
        Assert.True(panel.Gamma.IsFocused);
    }

    // Tabbing out of a control that is not itself a stop still lands on a real
    // one, so being focused outside the tab order is not a dead end.
    [AvaloniaFact]
    public void Tabbing_Away_From_Gamma_Rejoins_The_Order()
    {
        var (panel, window) = Shown();

        panel.Gamma.Focus();
        Dispatcher.UIThread.RunJobs();

        Tab(window);

        Assert.NotEqual("Gamma", Focused(window));
        Assert.Contains(Focused(window), new[] { "Alpha", "Beta", "Delta" });
    }

    [AvaloniaFact]
    public void MoveNext_Advances_The_Focus_Through_The_Focus_Manager()
    {
        var (panel, window) = Shown();

        panel.Beta.Focus();
        Dispatcher.UIThread.RunJobs();

        Assert.True(panel.MoveNext());
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Alpha", Focused(window));
    }

    [AvaloniaFact]
    public void MoveNext_Follows_The_Same_Order_As_Tab()
    {
        var (panel, window) = Shown();

        panel.Beta.Focus();
        Dispatcher.UIThread.RunJobs();

        var visited = new List<string>();

        for (var i = 0; i < 3; i++)
        {
            panel.MoveNext();
            Dispatcher.UIThread.RunJobs();
            visited.Add(Focused(window));
        }

        Assert.Equal(["Alpha", "Delta", "Beta"], visited);
    }

    // Guards against configuring the order by assigning TabIndex to Gamma too, or
    // by leaving a button at its default index of 0, which would sort ahead of
    // everything.
    [AvaloniaFact]
    public void The_Three_Stops_Carry_Ascending_Distinct_Tab_Indexes()
    {
        var (panel, _) = Shown();

        Assert.True(panel.Beta.TabIndex < panel.Alpha.TabIndex);
        Assert.True(panel.Alpha.TabIndex < panel.Delta.TabIndex);
        Assert.True(panel.Beta.IsTabStop);
        Assert.True(panel.Alpha.IsTabStop);
        Assert.True(panel.Delta.IsTabStop);
    }
}
