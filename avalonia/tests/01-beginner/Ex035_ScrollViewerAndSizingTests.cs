using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex035_ScrollViewerAndSizingTests
{
    private static Ex035_ScrollViewerAndSizing Show() =>
        ViewHarness.Show(new Ex035_ScrollViewerAndSizing(), 300, 300);

    // A fixed-height Border with no ScrollViewer at all could reproduce a
    // clipped LOOK, but it has neither an Extent nor a Viewport - a typed
    // FindControl<ScrollViewer> lookup by itself is the structural half of
    // this assertion, the Extent/Viewport relationship is the behavioural
    // half.
    [AvaloniaFact]
    public void Scroller_Content_Genuinely_Overflows_Its_Fixed_Viewport()
    {
        var view = Show();
        var scroller = view.FindControl<ScrollViewer>("Scroller")!;

        Assert.Equal(60, scroller.Viewport.Height);
        Assert.Equal(320, scroller.Extent.Height);
        Assert.True(scroller.Extent.Height > scroller.Viewport.Height);
    }

    [AvaloniaFact]
    public void Clamped_Borders_Width_And_Height_Are_Pinned_By_MinWidth_And_MaxHeight()
    {
        var view = Show();
        var clamped = view.FindControl<Border>("Clamped")!;

        Assert.Equal(250, clamped.MinWidth);
        Assert.Equal(40, clamped.MaxHeight);
        Assert.Equal(250, clamped.Bounds.Width);
        Assert.Equal(40, clamped.Bounds.Height);
    }
}
