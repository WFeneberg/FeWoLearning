using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex004_LayoutGridSpanTests
{
    // 400 wide over 1*/2*/1* gives 100 / 200 / 100.
    private static Ex004_LayoutGridSpan Show() =>
        ViewHarness.Show(new Ex004_LayoutGridSpan(), 400, 200);

    [AvaloniaFact]
    public void Banner_Spans_The_Full_Width_Of_All_Three_Columns()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 0, 400, 16), view.FindControl<Border>("Banner")!.Bounds);
    }

    // The discriminator: three equal columns would give 133.33 each and fail.
    [AvaloniaFact]
    public void Middle_Column_Is_Exactly_Twice_Each_Outer_Column()
    {
        var view = Show();

        var left = view.FindControl<Border>("Left")!;
        var middle = view.FindControl<Border>("Middle")!;
        var right = view.FindControl<Border>("Right")!;

        Assert.Equal(new Rect(0, 16, 100, 30), left.Bounds);
        Assert.Equal(new Rect(100, 16, 200, 30), middle.Bounds);
        Assert.Equal(new Rect(300, 16, 100, 30), right.Bounds);
        Assert.Equal(2 * left.Bounds.Width, middle.Bounds.Width);
    }
}
