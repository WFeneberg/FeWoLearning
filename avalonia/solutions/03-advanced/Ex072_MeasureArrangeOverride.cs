using System;
using Avalonia;
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex072_
public class Ex072_MeasureArrangeOverride : Control
{
    /// <summary>Given. Do not change.</summary>
    public static readonly StyledProperty<double> AspectRatioProperty =
        AvaloniaProperty.Register<Ex072_MeasureArrangeOverride, double>(
            nameof(AspectRatio), defaultValue: 2.0);

    /// <summary>Given. Do not change.</summary>
    public const double UnconstrainedWidth = 100.0;

    public double AspectRatio
    {
        get => GetValue(AspectRatioProperty);
        set => SetValue(AspectRatioProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var ratio = AspectRatio;
        var widthFree = double.IsInfinity(availableSize.Width);
        var heightFree = double.IsInfinity(availableSize.Height);

        // An infinite axis is not a very large axis: deriving a size from it
        // would hand the parent an infinite DesiredSize.
        var width = (widthFree, heightFree) switch
        {
            (true, true) => UnconstrainedWidth,
            (true, false) => availableSize.Height * ratio,
            (false, true) => availableSize.Width,
            (false, false) => Math.Min(availableSize.Width, availableSize.Height * ratio),
        };

        return new Size(width, width / ratio);
    }

    protected override Size ArrangeOverride(Size finalSize) => finalSize;
}
