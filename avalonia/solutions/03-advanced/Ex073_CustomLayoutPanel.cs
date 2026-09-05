using System;
using Avalonia;
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex073_
public class Ex073_CustomLayoutPanel : Panel
{
    /// <summary>Given. Do not change.</summary>
    public static readonly StyledProperty<double> StepProperty =
        AvaloniaProperty.Register<Ex073_CustomLayoutPanel, double>(
            nameof(Step), defaultValue: 5.0);

    static Ex073_CustomLayoutPanel() => AffectsMeasure<Ex073_CustomLayoutPanel>(StepProperty);

    public double Step
    {
        get => GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var step = Step;
        var width = 0.0;
        var height = 0.0;

        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[i];

            // DesiredSize is only meaningful after Measure, whatever the child
            // may already claim about its Width and Height.
            child.Measure(availableSize);

            var offset = i * step;
            width = Math.Max(width, offset + child.DesiredSize.Width);
            height = Math.Max(height, offset + child.DesiredSize.Height);
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var step = Step;

        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            var offset = i * step;
            child.Arrange(new Rect(new Point(offset, offset), child.DesiredSize));
        }

        return finalSize;
    }
}
