using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 075 - CustomBrushGradient (advanced).
/// Goal:   Build gradient brushes in code from DATA rather than from literals: a
///         heat legend whose stop offsets are computed by normalising measured
///         values onto 0..1, plus a radial brush meant to be used as an opacity
///         mask.
/// Drills: LinearGradientBrush, RadialGradientBrush, GradientStop offsets,
///         RelativePoint and RelativeUnit, SpreadMethod, OpacityMask.
/// Passes: dotnet test --filter FullyQualifiedName~Ex075_
///
/// The offsets are the exercise. A gradient's stops live on 0..1 regardless of
/// what the underlying numbers mean, so values of 10, 20 and 40 become offsets 0,
/// one third and 1 - the middle stop sits a third of the way along, not halfway,
/// because 20 is a third of the way from 10 to 40. Copying the values in as
/// offsets, or spacing the stops evenly, both look plausible and are both wrong.
///
/// Watch the degenerate inputs: one stop, or several stops that all carry the same
/// value, leave nothing to normalise against and divide by zero in the obvious
/// implementation.
public static class Ex075_CustomBrushGradient
{
    /// <summary>
    /// A left-to-right legend over the given readings, in the order supplied.
    ///
    /// Offsets are (value - min) / (max - min), so the smallest reading sits at 0
    /// and the largest at 1. When every reading is the same - and when there is
    /// only one - every offset is 0.
    ///
    /// The brush runs horizontally across whatever it fills: StartPoint at the
    /// relative left edge, EndPoint at the relative right edge, and SpreadMethod
    /// left at Pad.
    /// </summary>
    public static LinearGradientBrush BuildLegend(IReadOnlyList<Ex075_Reading> readings) =>
        throw new NotImplementedException(
            "TODO: Ex075 - a LinearGradientBrush from relative (0,0) to relative " +
            "(1,0) whose GradientStops carry each reading's Colour at its " +
            "normalised offset, guarding the case where min equals max");

    /// <summary>
    /// The brush a caller assigns to OpacityMask to fade a control out towards its
    /// edges: radial, centred, radius half the control on both axes, opaque white
    /// at offset 0 and fully transparent at offset 1.
    ///
    /// White is not decoration - an opacity mask uses only the ALPHA of what it
    /// paints, so the colour merely has to be opaque.
    /// </summary>
    public static RadialGradientBrush BuildEdgeFade() =>
        throw new NotImplementedException(
            "TODO: Ex075 - a RadialGradientBrush with Center and GradientOrigin at " +
            "relative (0.5, 0.5), RadiusX and RadiusY of 0.5, and stops of " +
            "Colors.White at 0 and Colors.Transparent at 1");
}

/// <summary>Given. Do not change. One measured reading and the colour it should show as.</summary>
public sealed record Ex075_Reading(double Value, Color Colour);
