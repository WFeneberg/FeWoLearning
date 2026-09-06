using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace FeWoLearning.Avalonia.Exercises.Expert;

// Passes: dotnet test --filter FullyQualifiedName~Ex098_
public static class Ex098_RenderedFrameCapture
{
    /// <summary>Given. Do not change.</summary>
    public static Color TopColour { get; } = Colors.Red;

    /// <summary>Given. Do not change.</summary>
    public static Color BottomColour { get; } = Colors.Blue;

    /// <summary>Given. Do not change.</summary>
    public const int HalfHeight = 15;

    /// <summary>Given. Do not change.</summary>
    public const int Width = 40;

    /// <summary>Given. Do not change.</summary>
    public static Control BuildTarget() =>
        new StackPanel
        {
            Children =
            {
                new Border { Background = new SolidColorBrush(TopColour), Width = Width, Height = HalfHeight },
                new Border { Background = new SolidColorBrush(BottomColour), Width = Width, Height = HalfHeight },
            },
        };

    public static Color PixelAt(WriteableBitmap frame, int x, int y)
    {
        using var locked = frame.Lock();
        var bytes = new byte[locked.RowBytes];

        // One row at a time: the offset of a row is y * RowBytes, which is not
        // necessarily y * width * 4.
        Marshal.Copy(locked.Address + (y * locked.RowBytes), bytes, 0, locked.RowBytes);

        var offset = x * 4;

        // Rgba8888: red first, alpha last.
        return Color.FromArgb(bytes[offset + 3], bytes[offset], bytes[offset + 1], bytes[offset + 2]);
    }

    public static int DistinctColours(WriteableBitmap frame)
    {
        var seen = new HashSet<Color>();

        for (var y = 0; y < frame.PixelSize.Height; y++)
        {
            for (var x = 0; x < frame.PixelSize.Width; x++)
            {
                seen.Add(PixelAt(frame, x, y));
            }
        }

        return seen.Count;
    }
}
