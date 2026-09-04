using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex006_AlignmentAndMarginTests
{
    private static Ex006_AlignmentAndMargin Show() =>
        ViewHarness.Show(new Ex006_AlignmentAndMargin(), 300, 200);

    // Frame is explicitly sized and must be pinned top-left, not centred in the
    // 300x200 control. Stretch alignment with an explicit Width centres it, which
    // is the mistake this asserts against.
    [AvaloniaFact]
    public void Frame_Is_Two_Hundred_By_One_Hundred_At_The_Top_Left()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 0, 200, 100), view.FindControl<Border>("Frame")!.Bounds);
    }

    // Content area is inset 10 on every side: x 10..190 (180 wide), y 10..90 (80 tall).
    // Centred horizontally: 10 + (180 - 40) / 2 = 80.
    // Bottom aligned with a 5 margin below: 10 + 80 - 20 - 5 = 65.
    [AvaloniaFact]
    public void Box_Is_Centred_Horizontally_And_Sits_Five_Above_The_Padded_Bottom()
    {
        var view = Show();

        Assert.Equal(new Rect(80, 65, 40, 20), view.FindControl<Border>("Box")!.Bounds);
    }

    // The discriminator: with default (Stretch) alignment and an explicit Width/Height,
    // a Box positioned by hand-picked asymmetric margins alone can land on the exact
    // same rectangle as the one above, without ever using HorizontalAlignment/
    // VerticalAlignment - the mechanism this exercise drills. Assert on the alignment
    // and margin properties themselves, not just the rendered rectangle.
    [AvaloniaFact]
    public void Box_Uses_Center_And_Bottom_Alignment_With_A_Five_Pixel_Bottom_Margin()
    {
        var view = Show();
        var box = view.FindControl<Border>("Box")!;

        Assert.Equal(HorizontalAlignment.Center, box.HorizontalAlignment);
        Assert.Equal(VerticalAlignment.Bottom, box.VerticalAlignment);
        Assert.Equal(new Thickness(0, 0, 0, 5), box.Margin);
    }

    // The Margin-vs-Padding discriminator: the 10-pixel inset around the Frame's
    // content must come from Padding, not from a same-looking Margin on Box or a
    // smaller explicit Frame content size.
    [AvaloniaFact]
    public void Frame_Applies_Its_Ten_Pixel_Padding()
    {
        var view = Show();

        Assert.Equal(new Thickness(10), view.FindControl<Border>("Frame")!.Padding);
    }
}
