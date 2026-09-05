using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex075_
public static class Ex075_CustomBrushGradient
{
    public static LinearGradientBrush BuildLegend(IReadOnlyList<Ex075_Reading> readings)
    {
        var brush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
            SpreadMethod = GradientSpreadMethod.Pad,
        };

        if (readings.Count == 0)
        {
            return brush;
        }

        var min = readings.Min(r => r.Value);
        var span = readings.Max(r => r.Value) - min;

        foreach (var reading in readings)
        {
            // A single reading, or several identical ones, leave no span to
            // normalise against: stack them all at the start rather than dividing
            // by zero.
            var offset = span == 0 ? 0 : (reading.Value - min) / span;
            brush.GradientStops.Add(new GradientStop(reading.Colour, offset));
        }

        return brush;
    }

    public static RadialGradientBrush BuildEdgeFade() =>
        new()
        {
            Center = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
            GradientOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
            RadiusX = new RelativeScalar(0.5, RelativeUnit.Relative),
            RadiusY = new RelativeScalar(0.5, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Colors.White, 0),
                new GradientStop(Colors.Transparent, 1),
            },
        };
}

/// <summary>Given. Do not change.</summary>
public sealed record Ex075_Reading(double Value, Color Colour);
