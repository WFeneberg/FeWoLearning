using System.Windows;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex049_WindowManagerSettingsTests : CaliburnViewContext
{
    // ShowDialogAndCloseAsync (used below) lives on CaliburnViewContext; here it is passed a
    // settings dictionary of its own rather than the invisible default one, because this
    // exercise IS about that dictionary.

    [WpfFact]
    public void BuildSettings_Contains_Exactly_The_Four_Requested_Values_Under_The_Right_Keys()
    {
        var settings = Ex049_WindowManagerSettings.BuildSettings("Hello", showInTaskbar: false, width: 321, left: -32000);

        // A stub that swapped Width/Left, or used a slightly wrong key name, is caught right
        // here - independent of whatever a live Window later does with them.
        Assert.Equal("Hello", settings["Title"]);
        Assert.Equal(false, settings["ShowInTaskbar"]);
        Assert.Equal(321.0, settings["Width"]);
        Assert.Equal(-32000.0, settings["Left"]);
    }

    [WpfFact]
    public async Task Title_And_ShowInTaskbar_Survive_Onto_The_Real_Hosting_Window()
    {
        var vm = new Screen();
        var settings = Ex049_WindowManagerSettings.BuildSettings("Hello", showInTaskbar: false, width: 321, left: -32000);

        var (_, window) = await ShowDialogAndCloseAsync(vm, true, settings);

        Assert.Equal("Hello", window.Title);
        Assert.False(window.ShowInTaskbar);
    }

    [WpfFact]
    public async Task Width_And_Left_Do_Not_Survive_Because_EnsureWindow_Overrides_Them_Afterwards()
    {
        var vm = new Screen();
        var settings = Ex049_WindowManagerSettings.BuildSettings("Hello", showInTaskbar: false, width: 321, left: -32000);

        var (_, window) = await ShowDialogAndCloseAsync(vm, true, settings);

        // The sharp half of this exercise: these two do NOT come back as requested.
        Assert.NotEqual(321.0, window.Width);
        Assert.NotEqual(-32000.0, window.Left);
        // The mechanism, not just the symptom: Caliburn's own SizeToContent/centring applied
        // afterwards is WHY - a settings dictionary that (say) forgot Width/Left entirely would
        // still make the two asserts above pass vacuously, so this pins down the actual cause.
        Assert.Equal(SizeToContent.WidthAndHeight, window.SizeToContent);
        Assert.Equal(WindowStartupLocation.CenterScreen, window.WindowStartupLocation);
    }

    [WpfFact]
    public async Task Different_Settings_Produce_Different_Titles_Proving_The_Values_Are_Not_Hardcoded()
    {
        var vm1 = new Screen();
        var (_, window1) = await ShowDialogAndCloseAsync(
            vm1, true, Ex049_WindowManagerSettings.BuildSettings("First", showInTaskbar: true, width: 100, left: 0));

        var vm2 = new Screen();
        var (_, window2) = await ShowDialogAndCloseAsync(
            vm2, true, Ex049_WindowManagerSettings.BuildSettings("Second", showInTaskbar: false, width: 200, left: 0));

        Assert.Equal("First", window1.Title);
        Assert.Equal("Second", window2.Title);
        Assert.True(window1.ShowInTaskbar);
        Assert.False(window2.ShowInTaskbar);
    }

    [WpfFact]
    public async Task An_Empty_Title_Still_Applies_Rather_Than_Being_Silently_Skipped()
    {
        var vm = new Screen();
        var settings = Ex049_WindowManagerSettings.BuildSettings("", showInTaskbar: true, width: 50, left: 0);

        var (_, window) = await ShowDialogAndCloseAsync(vm, true, settings);

        // A stub that only sets Title when it is non-empty (an easy, wrong "helpful" guard)
        // fails here - the dictionary always carries all four keys, unconditionally.
        Assert.Equal("", window.Title);
    }
}
