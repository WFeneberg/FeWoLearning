using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex071_
public class Ex071_CustomControlRender : Control
{
    /// <summary>Given. Do not change.</summary>
    public static readonly StyledProperty<IReadOnlyList<double>> ValuesProperty =
        AvaloniaProperty.Register<Ex071_CustomControlRender, IReadOnlyList<double>>(
            nameof(Values), defaultValue: []);

    /// <summary>Given. Do not change.</summary>
    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<Ex071_CustomControlRender, double>(
            nameof(StrokeThickness), defaultValue: 2.0);

    static Ex071_CustomControlRender() => AffectsRender<Ex071_CustomControlRender>(ValuesProperty);

    public IReadOnlyList<double> Values
    {
        get => GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public IReadOnlyList<Point> BuildPoints(Size size)
    {
        var values = Values;

        if (values.Count == 0)
        {
            return [];
        }

        if (values.Count == 1)
        {
            return [new Point(size.Width / 2, size.Height / 2)];
        }

        var min = values.Min();
        var span = values.Max() - min;
        var stepX = size.Width / (values.Count - 1);

        return values
            .Select((value, i) =>
            {
                // A flat series has no span to normalise against: park it on the
                // vertical middle rather than dividing by zero.
                var normalised = span == 0 ? 0.5 : (value - min) / span;
                return new Point(i * stepX, size.Height - (normalised * size.Height));
            })
            .ToList();
    }

    public override void Render(DrawingContext context)
    {
        var points = BuildPoints(Bounds.Size);

        if (points.Count < 2)
        {
            return;
        }

        var geometry = new StreamGeometry();

        using (var sink = geometry.Open())
        {
            sink.BeginFigure(points[0], isFilled: false);

            for (var i = 1; i < points.Count; i++)
            {
                sink.LineTo(points[i]);
            }

            sink.EndFigure(false);
        }

        context.DrawGeometry(brush: null, new Pen(Brushes.Black, StrokeThickness), geometry);
    }
}
