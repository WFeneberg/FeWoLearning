using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex062_CustomPanelTests : WpfTestContext
{
    // Records the exact constraint MeasureOverride actually passed down to it - the same
    // recording shape row 028's own element uses. Every OTHER test in this file uses a Border
    // with an explicit Width/Height, so a MeasureOverride that constrains height to availableSize
    // instead of leaving it unconstrained is invisible to them (an explicitly-sized child reports
    // the same DesiredSize either way); only reading the constraint directly, on an element with
    // no explicit size of its own, makes the instruction itself observable.
    private sealed class ConstraintProbeElement : FrameworkElement
    {
        public Size LastConstraint { get; private set; }
        public Size NaturalSize { get; set; } = new(10, 10);

        protected override Size MeasureOverride(Size constraint)
        {
            LastConstraint = constraint;
            return NaturalSize;
        }
    }

    private static Ex062_StackingPanel BuildPanel(params (double Width, double Height)[] children)
    {
        var panel = new Ex062_StackingPanel();
        foreach (var (width, height) in children)
        {
            panel.Children.Add(new Border { Width = width, Height = height });
        }

        return panel;
    }

    [WpfFact]
    public void DesiredSize_Is_Max_Width_And_Summed_Height_Of_The_Children()
    {
        var panel = BuildPanel((40, 20), (60, 30), (20, 50));

        Layout(panel);

        Assert.Equal(new Size(60, 100), panel.DesiredSize);
    }

    [WpfFact]
    public void DesiredSize_Recomputes_For_A_Different_Set_Of_Children()
    {
        // Vary the inputs from the first test - a mutant hard-coded to that one shape must not
        // survive a second, differently-sized tree.
        var panel = BuildPanel((10, 5), (10, 40));

        Layout(panel);

        Assert.Equal(new Size(10, 45), panel.DesiredSize);
    }

    [WpfFact]
    public void Children_Are_Arranged_At_A_Running_Y_Offset_Built_From_Each_Others_DesiredSize()
    {
        var panel = BuildPanel((40, 20), (60, 30), (20, 50));
        var children = panel.Children;

        Layout(panel);

        // Against "arrange them all at the origin": only the FIRST child may legitimately sit at
        // y=0 - the second and third must have moved down by the actual height of what came
        // before them, not a fixed increment.
        Assert.Equal(new Vector(0, 0), VisualTreeHelper.GetOffset(children[0]));
        Assert.Equal(new Vector(0, 20), VisualTreeHelper.GetOffset(children[1]));
        Assert.Equal(new Vector(0, 50), VisualTreeHelper.GetOffset(children[2]));
    }

    [WpfFact]
    public void Each_Child_Renders_At_Its_Own_DesiredSize_Not_Stretched_To_The_Panels_Width()
    {
        var panel = BuildPanel((40, 20), (60, 30), (20, 50));
        var children = panel.Children;

        Layout(panel, new Size(500, 500));

        Assert.Equal(new Size(40, 20), ((FrameworkElement)children[0]).RenderSize);
        Assert.Equal(new Size(60, 30), ((FrameworkElement)children[1]).RenderSize);
        Assert.Equal(new Size(20, 50), ((FrameworkElement)children[2]).RenderSize);
    }

    [WpfFact]
    public void Each_Child_Is_Measured_With_An_Unconstrained_Height()
    {
        var panel = new Ex062_StackingPanel();
        var probe = new ConstraintProbeElement { NaturalSize = new Size(30, 20) };
        panel.Children.Add(probe);

        // Deliberately a small available HEIGHT - against "child.Measure(availableSize)" instead
        // of an unconstrained height: that mutant would pass a finite (50) height straight
        // through, not PositiveInfinity.
        Layout(panel, new Size(200, 50));

        Assert.Equal(200, probe.LastConstraint.Width);
        Assert.True(double.IsPositiveInfinity(probe.LastConstraint.Height), $"expected an unconstrained height, got {probe.LastConstraint.Height}");
    }

    [WpfFact]
    public void ArrangeOverrides_Return_Is_Not_Clamped_To_A_Much_Larger_FinalSize()
    {
        var panel = BuildPanel((40, 20), (60, 30));

        // finalSize here (500,500) is far larger than the panel's own natural stacked size - a
        // mutant that returns finalSize verbatim from ArrangeOverride (instead of its own
        // computed size) would show RenderSize (500,500) instead.
        Layout(panel, new Size(500, 500));

        Assert.Equal(new Size(60, 50), panel.RenderSize);
    }
}
