using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex003_LayoutGridTests
{
    // 200 x 200: column 0 is a fixed 80, column 1 takes the remaining 120.
    // Row 0 is Auto and the header cells are 24 tall, so row 1 gets 176.
    private static Ex003_LayoutGrid Show() =>
        ViewHarness.Show(new Ex003_LayoutGrid(), 200, 200);

    [AvaloniaFact]
    public void Fixed_Column_Is_Eighty_And_The_Star_Column_Takes_The_Rest()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 0, 80, 24), view.FindControl<Border>("HeaderLeft")!.Bounds);
        Assert.Equal(new Rect(80, 0, 120, 24), view.FindControl<Border>("HeaderRight")!.Bounds);
    }

    [AvaloniaFact]
    public void Auto_Row_Takes_Its_Height_From_The_Header_And_The_Star_Row_Takes_The_Rest()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 24, 80, 176), view.FindControl<Border>("BodyLeft")!.Bounds);
        Assert.Equal(new Rect(80, 24, 120, 176), view.FindControl<Border>("BodyRight")!.Bounds);
    }
}
