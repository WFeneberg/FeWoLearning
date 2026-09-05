// Exercise 030 - Margin, Padding, and alignment (beginner). REFERENCE SOLUTION.
// Goal:   Tell apart the two kinds of space around an element - Margin, outside its own
//         box, and Padding, inside it - and see HorizontalAlignment/VerticalAlignment
//         decide whether an element fills the space it is handed or shrinks to its own
//         natural size and sits at one edge of it. Padding is not a FrameworkElement member
//         (Margin is) - it lives on Border, Control, TextBlock and a few others; this row
//         uses Border, wrapping a fixed-size child.
// Drills: FrameworkElement.Margin, Border.Padding, HorizontalAlignment, VerticalAlignment -
//         and the one outcome only the real mechanism produces: with the default Stretch,
//         RenderSize fills the space on offer (minus Margin); switch either alignment to
//         anything else and RenderSize shrinks to the element's own natural size instead,
//         sitting at one edge of that space rather than filling it.

using System.Windows;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex030_MarginPaddingAlignment
{
    /// <summary>
    /// Builds a Border with the given <paramref name="margin"/>, <paramref name="padding"/>,
    /// <paramref name="horizontal"/> and <paramref name="vertical"/> alignment, wrapping a
    /// fixed 40x20 child Border.
    /// </summary>
    public static Border BuildBorder(Thickness margin, Thickness padding, HorizontalAlignment horizontal, VerticalAlignment vertical)
    {
        return new Border
        {
            Margin = margin,
            Padding = padding,
            HorizontalAlignment = horizontal,
            VerticalAlignment = vertical,
            Child = new Border { Width = 40, Height = 20 },
        };
    }
}
