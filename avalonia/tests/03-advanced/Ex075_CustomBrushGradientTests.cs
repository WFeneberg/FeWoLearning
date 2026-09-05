using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex075_CustomBrushGradientTests
{
    private static LinearGradientBrush Legend(params (double Value, Color Colour)[] readings) =>
        Ex075_CustomBrushGradient.BuildLegend(
            readings.Select(r => new Ex075_Reading(r.Value, r.Colour)).ToList());

    // The arithmetic that is the point of the row: 20 is a third of the way from
    // 10 to 40, so its stop sits at a third - not at 0.5, which even spacing
    // would give, and not at 20, which copying the value in would give.
    [AvaloniaFact]
    public void Offsets_Are_The_Readings_Normalised_Onto_Zero_To_One()
    {
        var brush = Legend((10, Colors.Blue), (20, Colors.Yellow), (40, Colors.Red));

        var offsets = brush.GradientStops.Select(s => s.Offset).ToList();
        Assert.Equal(3, offsets.Count);
        Assert.Equal(0.0, offsets[0], precision: 9);
        Assert.Equal(1.0 / 3.0, offsets[1], precision: 9);
        Assert.Equal(1.0, offsets[2], precision: 9);
    }

    [AvaloniaFact]
    public void The_Colours_Are_Kept_In_The_Order_Supplied()
    {
        var brush = Legend((10, Colors.Blue), (20, Colors.Yellow), (40, Colors.Red));

        Assert.Equal([Colors.Blue, Colors.Yellow, Colors.Red], brush.GradientStops.Select(s => s.Color));
    }

    // Negative readings are the case that catches an implementation dividing by
    // the maximum instead of by the span.
    [AvaloniaFact]
    public void Readings_Below_Zero_Normalise_Just_The_Same()
    {
        var brush = Legend((-20, Colors.Blue), (-10, Colors.Red));

        Assert.Equal([0.0, 1.0], brush.GradientStops.Select(s => s.Offset));
    }

    [AvaloniaFact]
    public void A_Single_Reading_Sits_At_The_Start()
    {
        var brush = Legend((42, Colors.Red));

        Assert.Equal(0.0, Assert.Single(brush.GradientStops).Offset);
    }

    // Nothing to normalise against, so the obvious implementation divides by
    // zero and produces NaN offsets rather than throwing - which is worse,
    // because the brush then silently renders nothing.
    [AvaloniaFact]
    public void Identical_Readings_All_Sit_At_The_Start()
    {
        var brush = Legend((7, Colors.Blue), (7, Colors.Red));

        Assert.Equal([0.0, 0.0], brush.GradientStops.Select(s => s.Offset));
    }

    [AvaloniaFact]
    public void The_Legend_Runs_Horizontally_In_Relative_Units()
    {
        var brush = Legend((0, Colors.Blue), (1, Colors.Red));

        Assert.Equal(new RelativePoint(0, 0, RelativeUnit.Relative), brush.StartPoint);
        Assert.Equal(new RelativePoint(1, 0, RelativeUnit.Relative), brush.EndPoint);
        Assert.Equal(GradientSpreadMethod.Pad, brush.SpreadMethod);
    }

    [AvaloniaFact]
    public void The_Edge_Fade_Is_Centred_And_Runs_Opaque_To_Transparent()
    {
        var fade = Ex075_CustomBrushGradient.BuildEdgeFade();

        Assert.Equal(new RelativePoint(0.5, 0.5, RelativeUnit.Relative), fade.Center);
        Assert.Equal(new RelativeScalar(0.5, RelativeUnit.Relative), fade.RadiusX);
        Assert.Equal(new RelativeScalar(0.5, RelativeUnit.Relative), fade.RadiusY);
        Assert.Equal([Colors.White, Colors.Transparent], fade.GradientStops.Select(s => s.Color));
        Assert.Equal([0.0, 1.0], fade.GradientStops.Select(s => s.Offset));
    }

    // Both brushes have to survive being what they are for: a control really
    // taking them as Background and OpacityMask, laid out and rendered. What the
    // pixels became is not assertable here - see ex071's header for the three
    // measurements behind that - so this is a smoke check, and labelled as one.
    [AvaloniaFact]
    public void Both_Brushes_Can_Actually_Dress_A_Control()
    {
        var border = new Border
        {
            Width = 60,
            Height = 20,
            Background = Legend((0, Colors.Blue), (5, Colors.Red)),
            OpacityMask = Ex075_CustomBrushGradient.BuildEdgeFade(),
        };

        ViewHarness.Show(border, 100, 60);
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(new Rect(20, 20, 60, 20), border.Bounds);
    }
}
