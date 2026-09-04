using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex002_LayoutStackPanelTests
{
    private static (Border R1, Border R2, Border R3) Rows()
    {
        var view = ViewHarness.Show(new Ex002_LayoutStackPanel(), 200, 200);
        return (view.FindControl<Border>("Row1")!,
                view.FindControl<Border>("Row2")!,
                view.FindControl<Border>("Row3")!);
    }

    [AvaloniaFact]
    public void Each_Row_Is_Twenty_Tall_And_Fills_The_Width()
    {
        var (r1, r2, r3) = Rows();

        foreach (var row in new[] { r1, r2, r3 })
        {
            Assert.Equal(20, row.Bounds.Height);
            Assert.Equal(200, row.Bounds.Width);
        }
    }

    // The discriminator: any vertical arrangement puts the rows in order, but only
    // Spacing="8" produces exactly these offsets. A StackPanel with no Spacing
    // yields 0/20/40 and fails here.
    [AvaloniaFact]
    public void Rows_Are_Stacked_Top_Down_With_An_Eight_Pixel_Gap()
    {
        var (r1, r2, r3) = Rows();

        Assert.Equal(0, r1.Bounds.Y);
        Assert.Equal(28, r2.Bounds.Y);
        Assert.Equal(56, r3.Bounds.Y);
    }
}
