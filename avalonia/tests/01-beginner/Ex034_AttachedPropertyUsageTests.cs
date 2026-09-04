using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex034_AttachedPropertyUsageTests
{
    private static Ex034_AttachedPropertyUsage Show() =>
        ViewHarness.Show(new Ex034_AttachedPropertyUsage(), 300, 160);

    // The mechanism assertion: ToolTip.GetTip is the static getter for the
    // ATTACHED property. A code-behind assignment that mimics a tooltip
    // visually (e.g. a manually-shown Popup) never touches ToolTip.TipProperty
    // at all, so GetTip would return null against it.
    [AvaloniaFact]
    public void Hinted_Control_Has_Its_Tip_Set_Through_The_Attached_Property()
    {
        var view = Show();
        var hinted = view.FindControl<Control>("Hinted")!;

        Assert.Equal("a hint", ToolTip.GetTip(hinted));
    }

    // Guards against a cheat that sets a tip everywhere (e.g. a Style
    // targeting every Border) rather than the one named control.
    [AvaloniaFact]
    public void Plain_Control_Has_No_Tip_Set()
    {
        var view = Show();
        var plain = view.FindControl<Control>("Plain")!;

        Assert.Null(ToolTip.GetTip(plain));
    }
}
