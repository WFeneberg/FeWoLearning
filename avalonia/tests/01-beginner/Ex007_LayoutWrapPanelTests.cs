using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex007_LayoutWrapPanelTests
{
    private static Ex007_LayoutWrapPanel Show() =>
        ViewHarness.Show(new Ex007_LayoutWrapPanel(), 400, 200);

    [AvaloniaFact]
    public void First_Two_Tiles_Share_The_First_Row()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 0, 80, 20), view.FindControl<Border>("Item1")!.Bounds);
        Assert.Equal(new Rect(80, 0, 80, 20), view.FindControl<Border>("Item2")!.Bounds);
    }

    // The discriminator: a StackPanel or an unconstrained WrapPanel keeps all four on
    // one row, so Item3 would land at x=160 y=0 and fail here.
    [AvaloniaFact]
    public void Last_Two_Tiles_Wrap_Onto_A_Second_Row()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 20, 80, 20), view.FindControl<Border>("Item3")!.Bounds);
        Assert.Equal(new Rect(80, 20, 80, 20), view.FindControl<Border>("Item4")!.Bounds);
    }

    // The wrapping-mechanism discriminator: a Grid or Canvas with hand-picked
    // positions could reproduce the exact same four rectangles above without ever
    // wrapping. Look up the panel by name and confirm it really is a WrapPanel
    // constrained to 200 wide - the thing that actually forces the wrap.
    [AvaloniaFact]
    public void Items_Panel_Is_Actually_A_WrapPanel_Constrained_To_Two_Hundred_Wide()
    {
        var view = Show();
        var panel = view.FindControl<WrapPanel>("ItemsPanel");

        Assert.NotNull(panel);
        Assert.Equal(Orientation.Horizontal, panel!.Orientation);
        Assert.Equal(200, panel.Width);
    }
}
