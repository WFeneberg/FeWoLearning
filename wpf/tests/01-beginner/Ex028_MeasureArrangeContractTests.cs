using System.Windows;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex028_MeasureArrangeContractTests : WpfTestContext
{
    [WpfFact]
    public void MeasureOverride_Receives_The_Constraint_Reduced_By_Margin()
    {
        var element = new Ex028_MeasureArrangeElement { NaturalSize = new Size(30, 15), Margin = new Thickness(20) };

        element.Measure(new Size(300, 300));

        Assert.Equal(new Size(260, 260), element.LastMeasureConstraint);
    }

    [WpfFact]
    public void MeasureOverride_Receives_The_Constraint_Unchanged_When_There_Is_No_Margin()
    {
        var element = new Ex028_MeasureArrangeElement { NaturalSize = new Size(10, 10) };

        element.Measure(new Size(123, 77));

        Assert.Equal(new Size(123, 77), element.LastMeasureConstraint);
    }

    [WpfFact]
    public void DesiredSize_Is_The_Returned_NaturalSize_Plus_Margin()
    {
        var element = new Ex028_MeasureArrangeElement { NaturalSize = new Size(50, 30), Margin = new Thickness(10) };

        Layout(element, new Size(400, 400));

        Assert.Equal(new Size(70, 50), element.DesiredSize);
    }

    [WpfFact]
    public void RenderSize_Reflects_What_Was_Actually_Granted_Not_What_Was_Desired()
    {
        var element = new Ex028_MeasureArrangeElement { NaturalSize = new Size(50, 30), Margin = new Thickness(10) };

        Layout(element, new Size(400, 300));

        // DesiredSize is what the element wanted (NaturalSize + margin); RenderSize is what
        // it was actually granted, filling the space Arrange handed it - the two numbers
        // deliberately disagree here, which is the whole point of this row.
        Assert.Equal(new Size(70, 50), element.DesiredSize);
        Assert.Equal(new Size(380, 280), element.RenderSize);
        Assert.Equal(380, element.ActualWidth);
        Assert.Equal(280, element.ActualHeight);
    }

    [WpfFact]
    public void ArrangeOverride_Receives_A_FinalSize_Also_Reduced_By_Margin()
    {
        var element = new Ex028_MeasureArrangeElement { NaturalSize = new Size(20, 20), Margin = new Thickness(5, 15, 5, 15) };

        Layout(element, new Size(200, 200));

        Assert.Equal(new Size(190, 170), element.LastArrangeBounds);
        Assert.Equal(element.LastArrangeBounds, element.RenderSize);
    }
}
