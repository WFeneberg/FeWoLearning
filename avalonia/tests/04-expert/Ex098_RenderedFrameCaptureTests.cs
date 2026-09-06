using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using FeWoLearning.Avalonia.Tests;
using Ex = FeWoLearning.Avalonia.Exercises.Expert.Ex098_RenderedFrameCapture;

namespace FeWoLearning.Avalonia.Tests.Expert;

public class Ex098_RenderedFrameCaptureTests
{
    // One window per test, and a pumped frame before capturing: without the pump
    // there is nothing drawn to capture, because a headless test has no render
    // loop.
    private static WriteableBitmap Captured()
    {
        var window = ViewHarness.ShowWindow(Ex.BuildTarget(), Ex.Width, Ex.HalfHeight * 2);
        ViewHarness.PumpRender();

        var frame = window.CaptureRenderedFrame();
        Assert.NotNull(frame);
        return frame!;
    }

    // The frame covers the whole window, and its far corner is readable - the
    // second half is not padding: without it this test would only exercise the
    // harness and the given target, and would pass against the untouched stub.
    [AvaloniaFact]
    public void The_Frame_Is_The_Size_Of_The_Window_And_Readable_To_Its_Corner()
    {
        var frame = Captured();

        Assert.Equal(Ex.Width, frame.PixelSize.Width);
        Assert.Equal(Ex.HalfHeight * 2, frame.PixelSize.Height);
        Assert.Equal(Ex.BottomColour, Ex.PixelAt(frame, Ex.Width - 1, (Ex.HalfHeight * 2) - 1));
    }

    // The channel-order test, and the reason both halves are primaries rather
    // than shades of grey: read as BGRA instead of RGBA these two assertions swap
    // and both fail, which is exactly the mistake that a grey-only test would
    // never catch.
    [AvaloniaFact]
    public void The_Top_Half_Reads_Back_As_The_Colour_It_Was_Painted()
    {
        var frame = Captured();

        Assert.Equal(Ex.TopColour, Ex.PixelAt(frame, Ex.Width / 2, Ex.HalfHeight / 2));
    }

    [AvaloniaFact]
    public void The_Bottom_Half_Reads_Back_As_Its_Own_Colour()
    {
        var frame = Captured();

        Assert.Equal(Ex.BottomColour, Ex.PixelAt(frame, Ex.Width / 2, Ex.HalfHeight + (Ex.HalfHeight / 2)));
    }

    // Sampling more than one point per block, so a reader that happens to be
    // right in the middle of the frame and wrong elsewhere - a stride bug - shows
    // up. RowBytes is not necessarily width * 4.
    [AvaloniaFact]
    public void Both_Halves_Are_Solid_All_The_Way_Across()
    {
        var frame = Captured();

        foreach (var x in new[] { 0, 1, Ex.Width / 2, Ex.Width - 1 })
        {
            Assert.Equal(Ex.TopColour, Ex.PixelAt(frame, x, 1));
            Assert.Equal(Ex.BottomColour, Ex.PixelAt(frame, x, (Ex.HalfHeight * 2) - 1));
        }
    }

    // A stride bug also shows up as rows bleeding into each other, so the last
    // row of the top block and the first of the bottom are checked explicitly.
    [AvaloniaFact]
    public void The_Boundary_Between_The_Halves_Is_Where_It_Should_Be()
    {
        var frame = Captured();

        Assert.Equal(Ex.TopColour, Ex.PixelAt(frame, 5, Ex.HalfHeight - 1));
        Assert.Equal(Ex.BottomColour, Ex.PixelAt(frame, 5, Ex.HalfHeight));
    }

    // Two solid blocks and nothing else: no anti-aliased edge, no background
    // showing through. Measured on this harness, which is why the target is built
    // from full-width blocks rather than anything with a curve in it.
    [AvaloniaFact]
    public void The_Whole_Frame_Holds_Exactly_Two_Colours()
    {
        Assert.Equal(2, Ex.DistinctColours(Captured()));
    }

    // Every pixel is fully opaque, which pins the alpha byte down as well - a
    // reader that puts alpha in the wrong slot gets a transparent red here.
    [AvaloniaFact]
    public void The_Captured_Pixels_Are_Opaque()
    {
        var frame = Captured();

        Assert.Equal(byte.MaxValue, Ex.PixelAt(frame, 3, 3).A);
        Assert.Equal(byte.MaxValue, Ex.PixelAt(frame, 3, Ex.HalfHeight + 3).A);
    }
}
