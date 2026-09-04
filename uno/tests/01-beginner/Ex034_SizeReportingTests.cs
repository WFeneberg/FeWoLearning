using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex034_SizeReportingTests : UnoTestContext
{
    private static readonly Size Available = new(400, 200);

    [Fact]
    public void An_Explicit_Width_Is_Requested_Desired_And_Actual()
    {
        var report = Ex034_SizeReporting.Measure(new Border { Width = 30, Height = 20 }, Available);

        Assert.Equal(30, report.RequestedWidth, 1);
        Assert.Equal(30, report.DesiredWidth, 1);
        Assert.Equal(30, report.ActualWidth, 1);
        Assert.True(report.IsExplicit);
    }

    [Fact]
    public void An_Unset_Width_Is_Not_Zero_But_NaN()
    {
        var report = Ex034_SizeReporting.Measure(new Border(), Available);

        // The single most misread value in WinUI: Width is a request, and "no request" is
        // NaN. Comparing it to 0 is always false, including when you want it to be true.
        Assert.True(double.IsNaN(report.RequestedWidth));
        Assert.False(report.IsExplicit);
    }

    [Fact]
    public void An_Unconstrained_Element_Desires_Nothing_And_Gets_Everything()
    {
        var report = Ex034_SizeReporting.Measure(new Border(), Available);

        // Nothing inside it to measure, so it asks for nothing - and then Stretch hands it
        // the whole slot anyway.
        Assert.Equal(0, report.DesiredWidth, 1);
        Assert.Equal(400, report.ActualWidth, 1);
    }

    [Fact]
    public void Desired_Size_Includes_The_Margin()
    {
        var report = Ex034_SizeReporting.Measure(
            new Border { Width = 30, Height = 20, Margin = new Thickness(5) },
            Available);

        // 30 plus 5 on each side. DesiredSize is what the *parent* has to reserve, which
        // is why the margin is in it and not in ActualWidth.
        Assert.Equal(30, report.RequestedWidth, 1);
        Assert.Equal(40, report.DesiredWidth, 1);
        Assert.Equal(30, report.ActualWidth, 1);
    }

    [Fact]
    public void Content_Drives_The_Desired_Size_When_Nothing_Was_Requested()
    {
        var report = Ex034_SizeReporting.Measure(new TextBlock { Text = "Uno" }, Available);

        Assert.True(double.IsNaN(report.RequestedWidth));
        Assert.True(report.DesiredWidth > 0, "the text was not measured");
    }

    [Fact]
    public void A_Request_Bigger_Than_The_Slot_Is_Still_The_Request()
    {
        var report = Ex034_SizeReporting.Measure(new Border { Width = 900, Height = 20 }, Available);

        // The request survives untouched; DesiredSize does not. The measure pass clamps it
        // to what was available, so the parent is never told to reserve more than it has -
        // and the element is drawn at its 900 and clipped instead of shrunk.
        Assert.Equal(900, report.RequestedWidth, 1);
        Assert.Equal(400, report.DesiredWidth, 1);
    }

    [Fact]
    public void The_Report_Comes_From_A_Real_Arrange_Pass()
    {
        var element = new Border();

        var report = Ex034_SizeReporting.Measure(element, Available);

        // Measuring without arranging leaves ActualWidth at zero, and that zero is the
        // one people report as a bug.
        Assert.Equal(400, report.ActualWidth, 1);
        Assert.Equal(element.ActualWidth, report.ActualWidth, 1);
    }
}
