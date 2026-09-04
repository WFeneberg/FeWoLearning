using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex005_LayoutDockPanelTests
{
    private static Ex005_LayoutDockPanel Show() =>
        ViewHarness.Show(new Ex005_LayoutDockPanel(), 300, 200);

    [AvaloniaFact]
    public void Top_And_Bottom_Bars_Span_The_Full_Width()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 0, 300, 30), view.FindControl<Border>("TopBar")!.Bounds);
        Assert.Equal(new Rect(0, 180, 300, 20), view.FindControl<Border>("BottomBar")!.Bounds);
    }

    // The discriminator for dock ORDER: if SideBar were docked before TopBar it would
    // run the full 200 height instead of the 150 left between the bars.
    [AvaloniaFact]
    public void SideBar_Only_Occupies_What_Is_Left_Between_The_Bars()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 30, 60, 150), view.FindControl<Border>("SideBar")!.Bounds);
    }

    // Body relies on LastChildFill, which defaults to true in Avalonia 12.1.1: simply
    // omitting the attribute still fills the body. Only an explicit
    // LastChildFill="False" collapses Body to zero width, which is what this test
    // would catch.
    [AvaloniaFact]
    public void Body_Fills_The_Remaining_Space()
    {
        var view = Show();

        Assert.Equal(new Rect(60, 30, 240, 150), view.FindControl<Border>("Body")!.Bounds);
    }
}
