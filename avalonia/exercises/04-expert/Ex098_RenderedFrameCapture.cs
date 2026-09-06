using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace FeWoLearning.Avalonia.Exercises.Expert;

/// Exercise 098 - RenderedFrameCapture (expert).
/// Goal:   Read the pixels a window actually drew. Every other row in this track
///         asserts against the visual tree or the property system; this one goes
///         all the way to the frame and samples it.
/// Drills: TopLevel.CaptureRenderedFrame, WriteableBitmap.Lock, ILockedFramebuffer
///         with its Address and RowBytes, the Rgba8888 byte order.
/// Passes: dotnet test --filter FullyQualifiedName~Ex098_
///
/// THIS ROW IS WHY THE HARNESS CHANGED. CaptureRenderedFrame refuses outright
/// unless the app was built with .UseSkia() and UseHeadlessDrawing turned off -
/// measured, GetLastRenderedFrame throws NotSupportedException naming exactly
/// those two - so tests/_harness/TestAppHarness.cs now does both. Everything
/// below is measured against that harness.
///
/// TWO THINGS TO GET RIGHT, AND THE SECOND CATCHES EVERYONE.
///
/// Pump a frame first. There is no render loop in a headless test, so capture
/// after ViewHarness.PumpRender() or you are asking for a frame nobody drew.
///
/// THE FORMAT IS Rgba8888, SO BYTE 0 IS RED. Measured: a red Border filled a
/// 40x30 window with every pixel (255, 0, 0, 255) in memory order. Read it as
/// BGRA - the habit from most desktop APIs - and red and blue swap silently,
/// which no test that only ever uses grey would ever notice. That is why the test
/// below samples a red block and a blue one.
///
/// Stride is the other trap: rows are RowBytes apart, which is not necessarily
/// width * 4. Index with y * RowBytes + x * 4.
public static class Ex098_RenderedFrameCapture
{
    /// <summary>Given. Do not change. The top half of the view.</summary>
    public static Color TopColour { get; } = Colors.Red;

    /// <summary>Given. Do not change. The bottom half.</summary>
    public static Color BottomColour { get; } = Colors.Blue;

    /// <summary>Given. Do not change. Both halves are this tall.</summary>
    public const int HalfHeight = 15;

    /// <summary>Given. Do not change. And this wide.</summary>
    public const int Width = 40;

    /// <summary>
    /// Given. Do not change. Two solid blocks, stacked, filling a Width by
    /// 2 * HalfHeight area - chosen so every sampled pixel is unambiguous rather
    /// than an anti-aliased edge.
    /// </summary>
    public static Control BuildTarget() =>
        new StackPanel
        {
            Children =
            {
                new Border { Background = new SolidColorBrush(TopColour), Width = Width, Height = HalfHeight },
                new Border { Background = new SolidColorBrush(BottomColour), Width = Width, Height = HalfHeight },
            },
        };

    /// <summary>
    /// The colour of one pixel of <paramref name="frame"/>.
    ///
    /// Lock the bitmap, read four bytes at y * RowBytes + x * 4, and build a Color
    /// from them - remembering that the frame is Rgba8888, so those four bytes are
    /// red, green, blue, alpha in that order.
    ///
    /// Reading through the locked framebuffer needs unsafe code or
    /// System.Runtime.InteropServices.Marshal.Copy; the test project has no unsafe
    /// blocks enabled, so Marshal.Copy into a byte array is the route here.
    /// </summary>
    public static Color PixelAt(WriteableBitmap frame, int x, int y) =>
        throw new NotImplementedException(
            "TODO: Ex098 - frame.Lock(), Marshal.Copy the buffer out, index it at " +
            "y * RowBytes + x * 4, and return Color.FromArgb(a, r, g, b) with the " +
            "bytes in Rgba8888 order");

    /// <summary>
    /// How many distinct colours <paramref name="frame"/> contains, over its whole
    /// surface. For the target above, sampled after a frame has been drawn, this
    /// is exactly two.
    /// </summary>
    public static int DistinctColours(WriteableBitmap frame) =>
        throw new NotImplementedException(
            "TODO: Ex098 - count the distinct results of PixelAt over every pixel " +
            "of the frame");
}
