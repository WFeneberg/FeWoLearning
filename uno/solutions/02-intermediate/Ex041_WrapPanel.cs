// Exercise 041 - Wrap Panel (intermediate).
// Goal:   A complete panel: measure, break lines, arrange, and report an honest size.
// Drills: both overrides working from the same rule, line breaking against the available
//         width, and a DesiredSize that matches what the arrange pass will actually do.
// Passes: dotnet test --filter FullyQualifiedName~Ex041_

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Places children left to right at their desired size, starting a new line whenever the
/// next child would not fit. Each line is as tall as its tallest child.
/// </summary>
public partial class Ex041_WrapPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        var widest = 0d;
        var totalHeight = 0d;
        var lineWidth = 0d;
        var lineHeight = 0d;

        foreach (var child in Children)
        {
            child.Measure(availableSize);
            var size = child.DesiredSize;

            // The break test is "would this child overflow a line that already has
            // something on it". The lineWidth > 0 guard is what stops a child wider than
            // the whole line from breaking forever: it gets a line of its own, overflows
            // it, and the next child breaks after it.
            if (lineWidth > 0 && lineWidth + size.Width > availableSize.Width)
            {
                widest = Math.Max(widest, lineWidth);
                totalHeight += lineHeight;
                lineWidth = 0;
                lineHeight = 0;
            }

            lineWidth += size.Width;
            lineHeight = Math.Max(lineHeight, size.Height);
        }

        // The last line is still open when the loop ends.
        widest = Math.Max(widest, lineWidth);
        totalHeight += lineHeight;

        return new Size(widest, totalHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var x = 0d;
        var y = 0d;
        var lineHeight = 0d;

        foreach (var child in Children)
        {
            var size = child.DesiredSize;

            // Deliberately the same rule as above, against finalSize. If the two passes
            // ever disagree about where a line breaks, the panel reports a height it does
            // not use and the layout above it jitters on every re-measure.
            if (x > 0 && x + size.Width > finalSize.Width)
            {
                x = 0;
                y += lineHeight;
                lineHeight = 0;
            }

            child.Arrange(new Rect(x, y, size.Width, size.Height));
            x += size.Width;
            lineHeight = Math.Max(lineHeight, size.Height);
        }

        return finalSize;
    }
}
