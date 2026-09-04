using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Support;

/// <summary>
/// Test fixture: a deliberately naive vertical layout for <see cref="ItemsRepeater"/> that
/// realises every item. Not an exercise - writing one of these properly is ex071.
/// </summary>
/// <remarks>
/// Non-virtualising on purpose. A virtualising layout sizes its realisation window from the
/// effective viewport, and a windowless test tree has none, so it would realise a single
/// item and every assertion about the second one would be about the harness rather than
/// about the exercise.
/// </remarks>
public sealed class StackEverythingLayout : NonVirtualizingLayout
{
    protected override Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
    {
        double width = 0;
        double height = 0;

        foreach (var child in context.Children)
        {
            child.Measure(availableSize);
            width = Math.Max(width, child.DesiredSize.Width);
            height += child.DesiredSize.Height;
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
    {
        double y = 0;

        foreach (var child in context.Children)
        {
            child.Arrange(new Rect(0, y, finalSize.Width, child.DesiredSize.Height));
            y += child.DesiredSize.Height;
        }

        return finalSize;
    }
}
